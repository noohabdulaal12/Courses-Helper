using CoursesHelperMVC.Models;
using CoursesHelperMVC.Services;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Hubs;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace CoursesHelperMVC.Controllers;

[Authorize(Roles = "Coordinator")]
public class EnrollmentsController : Controller
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<EnrollmentHub> _hubContext;

    public EnrollmentsController(
        AppDbContext context,
        INotificationService notificationService,
        IHubContext<EnrollmentHub> hubContext)
    {
        _context = context;
        _notificationService = notificationService;
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        var enrollments = await _context.TraineeSessions
            .AsNoTracking()
            .Include(e => e.Trainee)
                .ThenInclude(t => t.UserInfo)
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Course)
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Classroom)
                    .ThenInclude(c => c.ClassroomType)
            .OrderBy(e => e.CourseSession.StartingDate)
            .ThenBy(e => e.CourseSession.StartingTime)
            .ThenBy(e => e.Trainee.UserName)
            .ToListAsync();

        return View(enrollments);
    }

    public async Task<IActionResult> Details(string? traineeId, int? sessionId)
    {
        if (string.IsNullOrWhiteSpace(traineeId) || sessionId is null)
        {
            return NotFound();
        }

        var enrollment = await FindEnrollmentAsync(traineeId, sessionId.Value, asNoTracking: true);
        return enrollment is null ? NotFound() : View(enrollment);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new EnrollmentFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnrollmentFormViewModel model)
    {
        await ValidateEnrollmentCreateAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var enrollment = new TraineeSession
        {
            TraineeId = model.TraineeId,
            SessionId = model.SessionId,
            AmountPaid = model.AmountPaid,
            PaymentDueDate = model.PaymentDueDate,
            Status = Status.Requested
        };

        _context.TraineeSessions.Add(enrollment);
        await _context.SaveChangesAsync();
        await SendSeatUpdateAsync(model.SessionId);

        var createdEnrollment = await FindEnrollmentAsync(model.TraineeId, model.SessionId, asNoTracking: true);
        if (createdEnrollment is not null)
        {
            await NotifyEnrollmentCreatedAsync(createdEnrollment);
        }

        TempData["SuccessMessage"] = "Enrollment created successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? traineeId, int? sessionId)
    {
        if (string.IsNullOrWhiteSpace(traineeId) || sessionId is null)
        {
            return NotFound();
        }

        var enrollment = await FindEnrollmentAsync(traineeId, sessionId.Value, asNoTracking: true);
        if (enrollment is null)
        {
            return NotFound();
        }

        PopulateStatuses(enrollment.Status);
        ViewBag.TraineeName = FormatTrainee(enrollment.Trainee);
        ViewBag.SessionDetails = FormatSession(enrollment.CourseSession);
        return View(ToFormModel(enrollment));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string traineeId, int sessionId, EnrollmentFormViewModel model)
    {
        if (traineeId != model.TraineeId || sessionId != model.SessionId)
        {
            return NotFound();
        }

        if (!Enum.IsDefined(typeof(Status), model.Status))
        {
            ModelState.AddModelError(nameof(EnrollmentFormViewModel.Status), "Please choose a valid status.");
        }

        await ValidatePaymentAsync(model);

        if (!ModelState.IsValid)
        {
            var current = await FindEnrollmentAsync(traineeId, sessionId, asNoTracking: true);
            if (current is null)
            {
                return NotFound();
            }

            PopulateStatuses(model.Status);
            ViewBag.TraineeName = FormatTrainee(current.Trainee);
            ViewBag.SessionDetails = FormatSession(current.CourseSession);
            return View(model);
        }

        var enrollment = await _context.TraineeSessions
            .FirstOrDefaultAsync(e => e.TraineeId == traineeId && e.SessionId == sessionId);

        if (enrollment is null)
        {
            return NotFound();
        }

        var previousAmountPaid = enrollment.AmountPaid;
        var previousPaymentDueDate = enrollment.PaymentDueDate;
        var previousStatus = enrollment.Status;

        enrollment.AmountPaid = model.AmountPaid;
        enrollment.PaymentDueDate = model.PaymentDueDate;
        enrollment.Status = model.Status;

        await SyncTraineeQualificationAsync(enrollment, previousStatus);

        await _context.SaveChangesAsync();

        var updatedEnrollment = await FindEnrollmentAsync(traineeId, sessionId, asNoTracking: true);
        if (updatedEnrollment is not null)
        {
            await NotifyEnrollmentUpdatedAsync(updatedEnrollment, previousStatus, previousAmountPaid, previousPaymentDueDate);
        }

        TempData["SuccessMessage"] = "Enrollment updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string? traineeId, int? sessionId)
    {
        if (string.IsNullOrWhiteSpace(traineeId) || sessionId is null)
        {
            return NotFound();
        }

        var enrollment = await FindEnrollmentAsync(traineeId, sessionId.Value, asNoTracking: true);
        return enrollment is null ? NotFound() : View(enrollment);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string traineeId, int sessionId)
    {
        var enrollment = await _context.TraineeSessions
            .FirstOrDefaultAsync(e => e.TraineeId == traineeId && e.SessionId == sessionId);

        if (enrollment is null)
        {
            return NotFound();
        }

        try
        {
            _context.TraineeSessions.Remove(enrollment);
            await _context.SaveChangesAsync();
            await SendSeatUpdateAsync(sessionId);
            TempData["SuccessMessage"] = "Enrollment deleted successfully.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "This enrollment could not be deleted because it is used by related records.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<TraineeSession?> FindEnrollmentAsync(string traineeId, int sessionId, bool asNoTracking)
    {
        var query = _context.TraineeSessions
            .Include(e => e.Trainee)
                .ThenInclude(t => t.UserInfo)
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Course)
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Classroom)
                    .ThenInclude(c => c.ClassroomType)
            .Where(e => e.TraineeId == traineeId && e.SessionId == sessionId);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync();
    }

    private async Task PopulateDropdownsAsync(EnrollmentFormViewModel? model = null)
    {
        var trainees = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserInfo)
            .Where(u => u.UserType == UserType.Trainee)
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var sessions = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Classroom)
                .ThenInclude(c => c.ClassroomType)
            .OrderBy(s => s.StartingDate)
            .ThenBy(s => s.StartingTime)
            .ToListAsync();

        ViewBag.Trainees = trainees
            .Select(t => new SelectListItem(FormatTrainee(t), t.Id, t.Id == model?.TraineeId))
            .ToList();

        ViewBag.CourseSessions = sessions
            .Select(s => new SelectListItem(FormatSession(s), s.Id.ToString(), s.Id == model?.SessionId))
            .ToList();
    }

    private void PopulateStatuses(Status selectedStatus)
    {
        ViewBag.Statuses = Enum.GetValues<Status>()
            .Select(status => new SelectListItem(FormatEnrollmentStatus(status), ((int)status).ToString(), status == selectedStatus))
            .ToList();
    }

    private async Task SyncTraineeQualificationAsync(TraineeSession enrollment, Status previousStatus)
    {
        var courseId = await _context.CourseSessions
            .Where(s => s.Id == enrollment.SessionId)
            .Select(s => s.CourseId)
            .FirstAsync();

        if (enrollment.Status == Status.Completed)
        {
            var exists = await _context.TraineeQualifications.AnyAsync(q =>
                q.TraineeId == enrollment.TraineeId &&
                q.CourseId == courseId);

            if (!exists)
            {
                _context.TraineeQualifications.Add(new TraineeQualification
                {
                    TraineeId = enrollment.TraineeId,
                    CourseId = courseId
                });
            }

            return;
        }

        if (previousStatus != Status.Completed)
        {
            return;
        }

        var hasOtherCompletedEnrollment = await _context.TraineeSessions.AnyAsync(e =>
            e.TraineeId == enrollment.TraineeId &&
            e.SessionId != enrollment.SessionId &&
            e.CourseSession.CourseId == courseId &&
            e.Status == Status.Completed);

        if (hasOtherCompletedEnrollment)
        {
            return;
        }

        var qualification = await _context.TraineeQualifications.FirstOrDefaultAsync(q =>
            q.TraineeId == enrollment.TraineeId &&
            q.CourseId == courseId);

        if (qualification is not null)
        {
            _context.TraineeQualifications.Remove(qualification);
        }
    }

    private async Task ValidateEnrollmentCreateAsync(EnrollmentFormViewModel model)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == model.TraineeId && u.UserType == UserType.Trainee))
        {
            ModelState.AddModelError(nameof(EnrollmentFormViewModel.TraineeId), "Please choose an existing trainee.");
        }

        var session = await _context.CourseSessions
            .Include(s => s.Course)
            .Include(s => s.TraineeSessions)
            .FirstOrDefaultAsync(s => s.Id == model.SessionId);

        if (session is null)
        {
            ModelState.AddModelError(nameof(EnrollmentFormViewModel.SessionId), "Please choose an existing course session.");
        }

        if (!ModelState.IsValid || session is null)
        {
            return;
        }

        var alreadyEnrolled = await _context.TraineeSessions.AnyAsync(e =>
            e.TraineeId == model.TraineeId &&
            e.SessionId == model.SessionId);

        if (alreadyEnrolled)
        {
            ModelState.AddModelError(string.Empty, "This trainee is already enrolled in the selected session.");
        }

        if (session.TraineeSessions.Count >= session.MaxSeats)
        {
            ModelState.AddModelError(nameof(EnrollmentFormViewModel.SessionId), "The selected session is full.");
        }

        await ValidatePaymentAsync(model, session.Course.Price);
    }

    private async Task ValidatePaymentAsync(EnrollmentFormViewModel model, decimal? coursePrice = null)
    {
        if (model.AmountPaid < 0)
        {
            ModelState.AddModelError(nameof(EnrollmentFormViewModel.AmountPaid), "Amount paid cannot be negative.");
        }

        coursePrice ??= await _context.CourseSessions
            .Where(s => s.Id == model.SessionId)
            .Select(s => (decimal?)s.Course.Price)
            .FirstOrDefaultAsync();

        if (coursePrice.HasValue && model.AmountPaid > coursePrice.Value)
        {
            ModelState.AddModelError(nameof(EnrollmentFormViewModel.AmountPaid), "Amount paid cannot exceed the course price.");
        }
    }

    private static EnrollmentFormViewModel ToFormModel(TraineeSession enrollment)
    {
        return new EnrollmentFormViewModel
        {
            TraineeId = enrollment.TraineeId,
            SessionId = enrollment.SessionId,
            AmountPaid = enrollment.AmountPaid,
            PaymentDueDate = enrollment.PaymentDueDate,
            Status = enrollment.Status
        };
    }

    public static string FormatTrainee(User trainee)
    {
        var name = trainee.UserInfo is null
            ? trainee.UserName ?? trainee.Email ?? trainee.Id
            : $"{trainee.UserInfo.FirstName} {trainee.UserInfo.LastName}";

        var email = trainee.Email ?? trainee.UserName;
        return string.IsNullOrWhiteSpace(email) || email == name ? name : $"{name} ({email})";
    }

    public static string FormatSession(CourseSession session)
    {
        return $"{session.Course.Name} on {session.StartingDate:yyyy-MM-dd} from {session.StartingTime:hh\\:mm} to {session.EndingTime:hh\\:mm}";
    }

    public static string FormatEnrollmentStatus(Status status)
    {
        return status switch
        {
            Status.Requested => "Enrolled / Requested",
            Status.Confirmed => "Confirmed",
            Status.Attending => "Attending",
            Status.Completed => "Completed / Passed",
            Status.Failed => "Failed",
            Status.Dropped => "Dropped",
            _ => status.ToString()
        };
    }

    private async Task NotifyEnrollmentCreatedAsync(TraineeSession enrollment)
    {
        var sessionDetails = FormatSession(enrollment.CourseSession);
        var traineeLink = Url.Action("MyEnrollments", "Trainee");
        var instructorLink = Url.Action("SessionTrainees", "Instructor", new { id = enrollment.SessionId });

        await _notificationService.NotifyUserAsync(
            enrollment.TraineeId,
            "Enrollment created",
            $"A coordinator enrolled you in {sessionDetails}.",
            traineeLink);

        await _notificationService.NotifyUserAsync(
            enrollment.CourseSession.InstructorId,
            "Enrollment added",
            $"{FormatTrainee(enrollment.Trainee)} was enrolled in {sessionDetails}.",
            instructorLink);
    }

    private async Task NotifyEnrollmentUpdatedAsync(
        TraineeSession enrollment,
        Status previousStatus,
        decimal previousAmountPaid,
        DateOnly? previousPaymentDueDate)
    {
        var sessionDetails = FormatSession(enrollment.CourseSession);
        var traineeLink = Url.Action("MyEnrollments", "Trainee");
        var instructorLink = Url.Action("SessionTrainees", "Instructor", new { id = enrollment.SessionId });

        if (previousStatus != enrollment.Status)
        {
            await _notificationService.NotifyUserAsync(
                enrollment.TraineeId,
                "Enrollment status updated",
                $"Your enrollment for {sessionDetails} is now {enrollment.Status}.",
                traineeLink);

            await _notificationService.NotifyUserAsync(
                enrollment.CourseSession.InstructorId,
                "Trainee status updated",
                $"{FormatTrainee(enrollment.Trainee)} is now {enrollment.Status} for {sessionDetails}.",
                instructorLink);
        }

        if (previousAmountPaid != enrollment.AmountPaid)
        {
            await _notificationService.NotifyUserAsync(
                enrollment.TraineeId,
                "Payment updated",
                $"Your payment record for {sessionDetails} was updated to {enrollment.AmountPaid:C}.",
                traineeLink);
        }

        if (previousPaymentDueDate != enrollment.PaymentDueDate)
        {
            var dueDateText = enrollment.PaymentDueDate?.ToString("yyyy-MM-dd") ?? "not set";
            await _notificationService.NotifyUserAsync(
                enrollment.TraineeId,
                "Payment due date updated",
                $"Your payment due date for {sessionDetails} is now {dueDateText}.",
                traineeLink);
        }
    }

    private async Task SendSeatUpdateAsync(int sessionId)
    {
        var sessionSeats = await _context.CourseSessions
            .Where(s => s.Id == sessionId)
            .Select(s => new
            {
                s.MaxSeats,
                Enrolled = s.TraineeSessions.Count
            })
            .FirstOrDefaultAsync();

        if (sessionSeats is null)
        {
            return;
        }

        await _hubContext.Clients.All.SendAsync(
            "ReceiveSeatUpdate",
            sessionId,
            sessionSeats.MaxSeats - sessionSeats.Enrolled);
    }
}

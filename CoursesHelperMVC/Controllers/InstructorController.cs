using CoursesHelperMVC.Models;
using CoursesHelperMVC.Services;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

[Authorize(Roles = "Instructor")]
public class InstructorController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly INotificationService _notificationService;

    public InstructorController(AppDbContext context, UserManager<User> userManager, INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Dashboard()
    {
        var instructorId = _userManager.GetUserId(User);
        if (instructorId is null)
        {
            return Challenge();
        }

        var sessions = await GetInstructorSessionsAsync(instructorId);
        var today = DateOnly.FromDateTime(DateTime.Today);

        return View(new InstructorDashboardViewModel
        {
            AssignedSessionCount = sessions.Count,
            UpcomingSessions = sessions
                .Where(s => s.StartingDate >= today)
                .Take(5)
                .ToList()
        });
    }

    public async Task<IActionResult> MySessions()
    {
        var instructorId = _userManager.GetUserId(User);
        if (instructorId is null)
        {
            return Challenge();
        }

        return View(await GetInstructorSessionsAsync(instructorId));
    }

    public async Task<IActionResult> SessionTrainees(int id)
    {
        var instructorId = _userManager.GetUserId(User);
        if (instructorId is null)
        {
            return Challenge();
        }

        var session = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.TraineeSessions)
                .ThenInclude(e => e.Trainee)
                    .ThenInclude(t => t.UserInfo)
            .FirstOrDefaultAsync(s => s.Id == id && s.InstructorId == instructorId);

        if (session is null)
        {
            return NotFound();
        }

        return View(new InstructorSessionTraineeViewModel
        {
            SessionId = session.Id,
            CourseName = session.Course.Name,
            Trainees = session.TraineeSessions
                .OrderBy(e => e.Trainee.UserName)
                .Select(e => new SessionTraineeViewModel
                {
                    TraineeId = e.TraineeId,
                    TraineeName = FormatUser(e.Trainee),
                    Email = e.Trainee.Email ?? e.Trainee.UserName ?? string.Empty,
                    Status = e.Status,
                    AmountPaid = e.AmountPaid
                })
                .ToList()
        });
    }

    public async Task<IActionResult> RecordAssessment(int sessionId, string traineeId)
    {
        var enrollment = await FindAssignedEnrollmentAsync(sessionId, traineeId, asNoTracking: true);
        if (enrollment is null)
        {
            return NotFound();
        }

        PopulateAssessmentStatuses(enrollment.Status);
        return View(new RecordAssessmentViewModel
        {
            SessionId = sessionId,
            TraineeId = traineeId,
            TraineeName = FormatUser(enrollment.Trainee),
            CourseName = enrollment.CourseSession.Course.Name,
            Status = enrollment.Status
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordAssessment(RecordAssessmentViewModel model)
    {
        if (model.Status is not Status.Completed and not Status.Failed and not Status.Dropped)
        {
            ModelState.AddModelError(nameof(model.Status), "Assessment result must be Pass, Fail, or Dropped.");
        }

        var enrollment = await FindAssignedEnrollmentAsync(model.SessionId, model.TraineeId, asNoTracking: false);
        if (enrollment is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            PopulateAssessmentStatuses(model.Status);
            model.TraineeName = FormatUser(enrollment.Trainee);
            model.CourseName = enrollment.CourseSession.Course.Name;
            return View(model);
        }

        var previousStatus = enrollment.Status;
        enrollment.Status = model.Status;

        if (model.Status == Status.Completed)
        {
            var courseId = enrollment.CourseSession.CourseId;
            var exists = await _context.TraineeQualifications.AnyAsync(q =>
                q.TraineeId == model.TraineeId &&
                q.CourseId == courseId);

            if (!exists)
            {
                _context.TraineeQualifications.Add(new TraineeQualification
                {
                    TraineeId = model.TraineeId,
                    CourseId = courseId
                });
            }
        }
        else if (previousStatus == Status.Completed)
        {
            await RemoveCompletedQualificationIfNoOtherCompletedEnrollmentAsync(enrollment);
        }

        await _context.SaveChangesAsync();

        await _notificationService.NotifyUserAsync(
            model.TraineeId,
            "Assessment recorded",
            $"Your assessment result for {enrollment.CourseSession.Course.Name} is {FormatAssessmentResult(model.Status)}.",
            Url.Action("MyEnrollments", "Trainee"));

        await _notificationService.NotifyCoordinatorsAsync(
            "Assessment recorded",
            $"{FormatUser(enrollment.Trainee)} was marked {FormatAssessmentResult(model.Status)} for {enrollment.CourseSession.Course.Name}.",
            Url.Action("Index", "Enrollments"));

        TempData["SuccessMessage"] = "Assessment updated successfully.";

        return RedirectToAction(nameof(SessionTrainees), new { id = model.SessionId });
    }

    private async Task<List<InstructorSessionViewModel>> GetInstructorSessionsAsync(string instructorId)
    {
        var sessions = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Classroom)
                .ThenInclude(c => c.ClassroomType)
            .Include(s => s.TraineeSessions)
            .Where(s => s.InstructorId == instructorId)
            .OrderBy(s => s.StartingDate)
            .ThenBy(s => s.StartingTime)
            .ToListAsync();

        return sessions.Select(s => new InstructorSessionViewModel
        {
            SessionId = s.Id,
            CourseName = s.Course.Name,
            ClassroomName = CourseSessionsController.FormatClassroom(s.Classroom),
            StartingDate = s.StartingDate,
            StartingTime = s.StartingTime,
            EndingTime = s.EndingTime,
            EnrolledCount = s.TraineeSessions.Count
        }).ToList();
    }

    private async Task<TraineeSession?> FindAssignedEnrollmentAsync(int sessionId, string traineeId, bool asNoTracking)
    {
        var instructorId = _userManager.GetUserId(User);
        if (instructorId is null)
        {
            return null;
        }

        var query = _context.TraineeSessions
            .Include(e => e.Trainee)
                .ThenInclude(t => t.UserInfo)
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Course)
            .Where(e =>
                e.SessionId == sessionId &&
                e.TraineeId == traineeId &&
                e.CourseSession.InstructorId == instructorId);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync();
    }

    private void PopulateAssessmentStatuses(Status selectedStatus)
    {
        ViewBag.Statuses = new List<SelectListItem>
        {
            new("Pass - mark enrollment completed", ((int)Status.Completed).ToString(), selectedStatus == Status.Completed),
            new("Fail - mark enrollment failed", ((int)Status.Failed).ToString(), selectedStatus == Status.Failed),
            new("Dropped - trainee did not complete", ((int)Status.Dropped).ToString(), selectedStatus == Status.Dropped)
        };
    }

    private async Task RemoveCompletedQualificationIfNoOtherCompletedEnrollmentAsync(TraineeSession enrollment)
    {
        var courseId = enrollment.CourseSession.CourseId;
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

    public static string FormatAssessmentResult(Status status)
    {
        return status switch
        {
            Status.Completed => "Pass",
            Status.Failed => "Fail",
            Status.Dropped => "Dropped",
            _ => status.ToString()
        };
    }

    private static string FormatUser(User user)
    {
        return user.UserInfo is null
            ? user.UserName ?? user.Email ?? user.Id
            : $"{user.UserInfo.FirstName} {user.UserInfo.LastName}";
    }
}

using CoursesHelperMVC.Models;
using CoursesHelperMVC.Services;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

[Authorize(Roles = "Trainee")]
public class TraineeController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly INotificationService _notificationService;

    public TraineeController(AppDbContext context, UserManager<User> userManager, INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Dashboard()
    {
        var traineeId = _userManager.GetUserId(User);
        if (traineeId is null)
        {
            return Challenge();
        }

        var enrollments = await GetTraineeEnrollmentsAsync(traineeId);
        var certificationProgress = await BuildCertificationProgressAsync(traineeId);
        var activeEnrollments = enrollments
            .Where(e => e.Status is not Status.Completed and not Status.Failed)
            .ToList();

        return View(new TraineeDashboardViewModel
        {
            ActiveEnrollmentCount = activeEnrollments.Count,
            OutstandingBalance = enrollments.Sum(e => e.RemainingBalance),
            EligibleCertificationCount = certificationProgress.Count(c => c.IsEligible),
            TotalCertificationCount = certificationProgress.Count,
            ActiveEnrollments = activeEnrollments
        });
    }

    public async Task<IActionResult> AvailableSessions()
    {
        var traineeId = _userManager.GetUserId(User);
        if (traineeId is null)
        {
            return Challenge();
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var enrolledSessionIds = await _context.TraineeSessions
            .AsNoTracking()
            .Where(e => e.TraineeId == traineeId)
            .Select(e => e.SessionId)
            .ToListAsync();

        var sessions = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
                .ThenInclude(c => c.Subject)
            .Include(s => s.Instructor)
                .ThenInclude(i => i.UserInfo)
            .Include(s => s.Classroom)
                .ThenInclude(c => c.ClassroomType)
            .Include(s => s.TraineeSessions)
            .Where(s => s.StartingDate >= today)
            .OrderBy(s => s.StartingDate)
            .ThenBy(s => s.StartingTime)
            .ToListAsync();

        var model = sessions.Select(s => new AvailableSessionViewModel
        {
            SessionId = s.Id,
            CourseName = s.Course.Name,
            SubjectName = s.Course.Subject.Name,
            InstructorName = FormatUser(s.Instructor),
            ClassroomName = CourseSessionsController.FormatClassroom(s.Classroom),
            StartingDate = s.StartingDate,
            StartingTime = s.StartingTime,
            EndingTime = s.EndingTime,
            MaxSeats = s.MaxSeats,
            CurrentEnrollments = s.TraineeSessions.Count,
            Price = s.Course.Price,
            IsAlreadyEnrolled = enrolledSessionIds.Contains(s.Id)
        }).ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int sessionId)
    {
        var traineeId = _userManager.GetUserId(User);
        if (traineeId is null)
        {
            return Challenge();
        }

        var session = await _context.CourseSessions
            .Include(s => s.Course)
                .ThenInclude(c => c.CoursePrerequisites)
            .Include(s => s.TraineeSessions)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null)
        {
            TempData["ErrorMessage"] = "The selected session was not found.";
            return RedirectToAction(nameof(AvailableSessions));
        }

        if (session.StartingDate < DateOnly.FromDateTime(DateTime.Today))
        {
            TempData["ErrorMessage"] = "You cannot enroll in a past session.";
            return RedirectToAction(nameof(AvailableSessions));
        }

        if (session.TraineeSessions.Any(e => e.TraineeId == traineeId))
        {
            TempData["ErrorMessage"] = "You are already enrolled in this session.";
            return RedirectToAction(nameof(AvailableSessions));
        }

        if (session.TraineeSessions.Count >= session.MaxSeats)
        {
            TempData["ErrorMessage"] = "This session is full.";
            return RedirectToAction(nameof(AvailableSessions));
        }

        var prerequisiteIds = session.Course.CoursePrerequisites.Select(p => p.PrerequisiteId).ToList();
        if (prerequisiteIds.Count > 0)
        {
            var completedIds = await _context.TraineeQualifications
                .Where(q => q.TraineeId == traineeId && prerequisiteIds.Contains(q.CourseId))
                .Select(q => q.CourseId)
                .ToListAsync();

            if (prerequisiteIds.Except(completedIds).Any())
            {
                TempData["ErrorMessage"] = "You must complete the prerequisite course before enrolling.";
                return RedirectToAction(nameof(AvailableSessions));
            }
        }

        _context.TraineeSessions.Add(new TraineeSession
        {
            TraineeId = traineeId,
            SessionId = sessionId,
            AmountPaid = 0,
            Status = Status.Requested
        });

        await _context.SaveChangesAsync();

        var sessionLink = Url.Action(nameof(MyEnrollments), "Trainee");
        await _notificationService.NotifyUserAsync(
            traineeId,
            "Enrollment request submitted",
            $"Your enrollment request for {session.Course.Name} was submitted.",
            sessionLink);

        await _notificationService.NotifyUserAsync(
            session.InstructorId,
            "New trainee enrollment",
            $"A trainee requested enrollment in your {session.Course.Name} session.",
            Url.Action("SessionTrainees", "Instructor", new { id = sessionId }));

        await _notificationService.NotifyCoordinatorsAsync(
            "New enrollment request",
            $"A trainee requested enrollment in {session.Course.Name}.",
            Url.Action("Index", "Enrollments"));

        TempData["SuccessMessage"] = "Enrollment request created successfully.";

        return RedirectToAction(nameof(MyEnrollments));
    }

    public async Task<IActionResult> MyEnrollments()
    {
        var traineeId = _userManager.GetUserId(User);
        if (traineeId is null)
        {
            return Challenge();
        }

        return View(await GetTraineeEnrollmentsAsync(traineeId));
    }

    public async Task<IActionResult> CertificationProgress()
    {
        var traineeId = _userManager.GetUserId(User);
        if (traineeId is null)
        {
            return Challenge();
        }

        return View(await BuildCertificationProgressAsync(traineeId));
    }

    private async Task<List<TraineeEnrollmentViewModel>> GetTraineeEnrollmentsAsync(string traineeId)
    {
        var enrollments = await _context.TraineeSessions
            .AsNoTracking()
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Course)
            .Where(e => e.TraineeId == traineeId)
            .OrderBy(e => e.CourseSession.StartingDate)
            .ThenBy(e => e.CourseSession.StartingTime)
            .ToListAsync();

        return enrollments.Select(e => new TraineeEnrollmentViewModel
        {
            SessionId = e.SessionId,
            CourseName = e.CourseSession.Course.Name,
            StartingDate = e.CourseSession.StartingDate,
            StartingTime = e.CourseSession.StartingTime,
            EndingTime = e.CourseSession.EndingTime,
            Status = e.Status,
            CoursePrice = e.CourseSession.Course.Price,
            AmountPaid = e.AmountPaid
        }).ToList();
    }

    private async Task<List<CertificationProgressViewModel>> BuildCertificationProgressAsync(string traineeId)
    {
        var completedCourseIds = await _context.TraineeQualifications
            .AsNoTracking()
            .Where(q => q.TraineeId == traineeId)
            .Select(q => q.CourseId)
            .ToListAsync();

        var certifications = await _context.Certifications
            .AsNoTracking()
            .Include(c => c.CertificationCourses)
                .ThenInclude(cc => cc.Course)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return certifications.Select(c =>
        {
            var required = c.CertificationCourses
                .OrderBy(cc => cc.Course.Name)
                .Select(cc => cc.Course)
                .ToList();

            return new CertificationProgressViewModel
            {
                CertificationName = c.Name,
                RequiredCourses = required.Select(course => course.Name).ToList(),
                CompletedCourses = required.Where(course => completedCourseIds.Contains(course.Id)).Select(course => course.Name).ToList(),
                PendingCourses = required.Where(course => !completedCourseIds.Contains(course.Id)).Select(course => course.Name).ToList()
            };
        }).ToList();
    }

    private static string FormatUser(User user)
    {
        return user.UserInfo is null
            ? user.UserName ?? user.Email ?? user.Id
            : $"{user.UserInfo.FirstName} {user.UserInfo.LastName}";
    }
}

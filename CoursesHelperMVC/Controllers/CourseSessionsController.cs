using CoursesHelperMVC.Models;
using CoursesHelperMVC.Services;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

[Authorize(Roles = "Coordinator")]
public class CourseSessionsController : Controller
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public CourseSessionsController(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        var sessions = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
                .ThenInclude(i => i.UserInfo)
            .Include(s => s.Classroom)
                .ThenInclude(c => c.ClassroomType)
            .Include(s => s.TraineeSessions)
            .OrderBy(s => s.StartingDate)
            .ThenBy(s => s.StartingTime)
            .ToListAsync();

        return View(sessions);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var session = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
                .ThenInclude(i => i.UserInfo)
            .Include(s => s.Classroom)
                .ThenInclude(c => c.ClassroomType)
            .Include(s => s.TraineeSessions)
            .FirstOrDefaultAsync(s => s.Id == id);

        return session is null ? NotFound() : View(session);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CourseSessionFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseSessionFormViewModel model)
    {
        await ValidateSessionAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var session = new CourseSession
        {
            CourseId = model.CourseId,
            InstructorId = model.InstructorId,
            ClassroomId = model.ClassroomId,
            MaxSeats = model.MaxSeats,
            StartingDate = model.StartingDate,
            StartingTime = model.StartingTime,
            EndingTime = model.EndingTime,
            DayOfWeek = model.StartingDate.DayOfWeek
        };

        _context.CourseSessions.Add(session);
        await _context.SaveChangesAsync();

        await NotifySessionCreatedAsync(session.Id);
        TempData["SuccessMessage"] = "Course session created successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var session = await _context.CourseSessions.FindAsync(id);
        if (session is null)
        {
            return NotFound();
        }

        var model = ToFormModel(session);
        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseSessionFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        await ValidateSessionAsync(model, id);

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var session = await _context.CourseSessions.FindAsync(id);
        if (session is null)
        {
            return NotFound();
        }

        var previousInstructorId = session.InstructorId;

        session.CourseId = model.CourseId;
        session.InstructorId = model.InstructorId;
        session.ClassroomId = model.ClassroomId;
        session.MaxSeats = model.MaxSeats;
        session.StartingDate = model.StartingDate;
        session.StartingTime = model.StartingTime;
        session.EndingTime = model.EndingTime;
        session.DayOfWeek = model.StartingDate.DayOfWeek;

        try
        {
            await _context.SaveChangesAsync();
            await NotifySessionUpdatedAsync(id, previousInstructorId);
            TempData["SuccessMessage"] = "Course session updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CourseSessionExistsAsync(id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var session = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
                .ThenInclude(i => i.UserInfo)
            .Include(s => s.Classroom)
                .ThenInclude(c => c.ClassroomType)
            .FirstOrDefaultAsync(s => s.Id == id);

        return session is null ? NotFound() : View(session);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var session = await _context.CourseSessions.FindAsync(id);
        if (session is null)
        {
            return NotFound();
        }

        try
        {
            var instructorId = session.InstructorId;
            var courseName = await _context.Courses
                .Where(c => c.Id == session.CourseId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync() ?? "a course";

            _context.CourseSessions.Remove(session);
            await _context.SaveChangesAsync();

            await _notificationService.NotifyUserAsync(
                instructorId,
                "Session removed",
                $"Your session for {courseName} was removed from the schedule.",
                Url.Action(nameof(Index), "CourseSessions"));

            TempData["SuccessMessage"] = "Course session deleted successfully.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "This course session cannot be deleted because it is used by related enrollment records.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(CourseSessionFormViewModel? model = null)
    {
        var courses = await _context.Courses
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        var instructors = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserInfo)
            .Where(u => u.UserType == UserType.Instructor)
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var classrooms = await _context.Classrooms
            .AsNoTracking()
            .Include(c => c.ClassroomType)
            .OrderBy(c => c.ClassroomType.Name)
            .ThenBy(c => c.Id)
            .ToListAsync();

        ViewBag.Courses = new SelectList(courses, nameof(Course.Id), nameof(Course.Name), model?.CourseId);
        ViewBag.Instructors = instructors
            .Select(i => new SelectListItem(FormatInstructor(i), i.Id, i.Id == model?.InstructorId))
            .ToList();
        ViewBag.Classrooms = classrooms
            .Select(c => new SelectListItem(FormatClassroom(c), c.Id.ToString(), c.Id == model?.ClassroomId))
            .ToList();
    }

    private async Task ValidateSessionAsync(CourseSessionFormViewModel model, int? sessionId = null)
    {
        if (model.StartingDate < DateOnly.FromDateTime(DateTime.Today))
        {
            ModelState.AddModelError(nameof(CourseSessionFormViewModel.StartingDate), "Starting date cannot be in the past.");
        }

        if (model.EndingTime <= model.StartingTime)
        {
            ModelState.AddModelError(nameof(CourseSessionFormViewModel.EndingTime), "Ending time must be after starting time.");
        }

        if (!await _context.Courses.AnyAsync(c => c.Id == model.CourseId))
        {
            ModelState.AddModelError(nameof(CourseSessionFormViewModel.CourseId), "Please choose an existing course.");
        }

        if (!await _context.Users.AnyAsync(u => u.Id == model.InstructorId && u.UserType == UserType.Instructor))
        {
            ModelState.AddModelError(nameof(CourseSessionFormViewModel.InstructorId), "Please choose an existing instructor.");
        }

        if (!await _context.Classrooms.AnyAsync(c => c.Id == model.ClassroomId))
        {
            ModelState.AddModelError(nameof(CourseSessionFormViewModel.ClassroomId), "Please choose an existing classroom.");
        }

        if (!ModelState.IsValid)
        {
            return;
        }

        var classroomConflict = await _context.CourseSessions.AnyAsync(s =>
            s.Id != sessionId &&
            s.ClassroomId == model.ClassroomId &&
            s.StartingDate == model.StartingDate &&
            model.StartingTime < s.EndingTime &&
            model.EndingTime > s.StartingTime);

        if (classroomConflict)
        {
            ModelState.AddModelError(string.Empty, "This classroom is already booked during the selected time.");
        }

        var instructorConflict = await _context.CourseSessions.AnyAsync(s =>
            s.Id != sessionId &&
            s.InstructorId == model.InstructorId &&
            s.StartingDate == model.StartingDate &&
            model.StartingTime < s.EndingTime &&
            model.EndingTime > s.StartingTime);

        if (instructorConflict)
        {
            ModelState.AddModelError(string.Empty, "This instructor is already booked during the selected time.");
        }

        var isQualified = await _context.instructorQualifications.AnyAsync(q =>
            q.InstructorId == model.InstructorId &&
            q.CourseId == model.CourseId);

        if (!isQualified)
        {
            ModelState.AddModelError(nameof(CourseSessionFormViewModel.InstructorId), "This instructor is not qualified to teach the selected course.");
        }

        var isAvailable = await _context.InstructorAvailabilities.AnyAsync(a =>
            a.InstructorId == model.InstructorId &&
            a.DayOfWeek == model.StartingDate.DayOfWeek &&
            a.StartingTime <= model.StartingTime &&
            a.EndingTime >= model.EndingTime);

        if (!isAvailable)
        {
            ModelState.AddModelError(nameof(CourseSessionFormViewModel.InstructorId), "This instructor is not available during the selected session time.");
        }
    }

    private async Task<bool> CourseSessionExistsAsync(int id)
    {
        return await _context.CourseSessions.AnyAsync(s => s.Id == id);
    }

    private static CourseSessionFormViewModel ToFormModel(CourseSession session)
    {
        return new CourseSessionFormViewModel
        {
            Id = session.Id,
            CourseId = session.CourseId,
            InstructorId = session.InstructorId,
            ClassroomId = session.ClassroomId,
            MaxSeats = session.MaxSeats,
            StartingDate = session.StartingDate,
            StartingTime = session.StartingTime,
            EndingTime = session.EndingTime
        };
    }

    public static string FormatInstructor(CoursesHelperWebAPI.Models.Identity.User instructor)
    {
        if (instructor.UserInfo is not null)
        {
            return $"{instructor.UserInfo.FirstName} {instructor.UserInfo.LastName}";
        }

        return instructor.UserName ?? instructor.Email ?? instructor.Id;
    }

    public static string FormatClassroom(Classroom classroom)
    {
        var typeName = classroom.ClassroomType?.Name ?? "Classroom";
        var description = string.IsNullOrWhiteSpace(classroom.Description)
            ? string.Empty
            : $" - {classroom.Description}";

        return $"{typeName} #{classroom.Id}{description}";
    }

    private async Task NotifySessionCreatedAsync(int sessionId)
    {
        var session = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null)
        {
            return;
        }

        await _notificationService.NotifyUserAsync(
            session.InstructorId,
            "New session assigned",
            $"You were assigned to teach {FormatSessionTitle(session)}.",
            Url.Action("MySessions", "Instructor"));
    }

    private async Task NotifySessionUpdatedAsync(int sessionId, string previousInstructorId)
    {
        var session = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.TraineeSessions)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null)
        {
            return;
        }

        await _notificationService.NotifyUserAsync(
            session.InstructorId,
            "Session schedule updated",
            $"Your session for {FormatSessionTitle(session)} was updated.",
            Url.Action("MySessions", "Instructor"));

        if (previousInstructorId != session.InstructorId)
        {
            await _notificationService.NotifyUserAsync(
                previousInstructorId,
                "Session reassigned",
                $"You are no longer assigned to teach {FormatSessionTitle(session)}.",
                Url.Action("MySessions", "Instructor"));
        }

        foreach (var enrollment in session.TraineeSessions)
        {
            await _notificationService.NotifyUserAsync(
                enrollment.TraineeId,
                "Session schedule updated",
                $"Your session for {FormatSessionTitle(session)} was updated.",
                Url.Action("MyEnrollments", "Trainee"));
        }
    }

    private static string FormatSessionTitle(CourseSession session)
    {
        return $"{session.Course.Name} on {session.StartingDate:yyyy-MM-dd} from {session.StartingTime:hh\\:mm} to {session.EndingTime:hh\\:mm}";
    }
}

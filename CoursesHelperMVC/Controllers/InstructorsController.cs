using CoursesHelperMVC.Models;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

[Authorize(Roles = "Coordinator")]
public class InstructorsController : Controller
{
    private readonly AppDbContext _context;

    public InstructorsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var instructors = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserInfo)
            .Include(u => u.InstructorQualifications)
            .Include(u => u.InstructorAvailabilities)
            .Include(u => u.CourseSessions)
            .Where(u => u.UserType == UserType.Instructor)
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var model = instructors.Select(i => new InstructorListItemViewModel
        {
            InstructorId = i.Id,
            Name = FormatUser(i),
            Email = i.Email ?? i.UserName ?? string.Empty,
            QualifiedCourseCount = i.InstructorQualifications.Count,
            AvailabilitySlotCount = i.InstructorAvailabilities.Count,
            AssignedSessionCount = i.CourseSessions.Count
        }).ToList();

        return View(model);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var instructor = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserInfo)
            .Include(u => u.InstructorQualifications)
                .ThenInclude(q => q.Course)
                    .ThenInclude(c => c.Subject)
            .Include(u => u.InstructorAvailabilities)
            .Include(u => u.CourseSessions)
                .ThenInclude(s => s.Course)
            .Include(u => u.CourseSessions)
                .ThenInclude(s => s.Classroom)
                    .ThenInclude(c => c.ClassroomType)
            .Include(u => u.CourseSessions)
                .ThenInclude(s => s.TraineeSessions)
            .FirstOrDefaultAsync(u => u.Id == id && u.UserType == UserType.Instructor);

        if (instructor is null)
        {
            return NotFound();
        }

        return View(new InstructorDetailsViewModel
        {
            InstructorId = instructor.Id,
            Name = FormatUser(instructor),
            Email = instructor.Email ?? instructor.UserName ?? string.Empty,
            DateOfBirth = instructor.UserInfo?.DateOfBirth,
            QualifiedCourses = instructor.InstructorQualifications
                .OrderBy(q => q.Course.Subject.Name)
                .ThenBy(q => q.Course.Name)
                .Select(q => $"{q.Course.Name} ({q.Course.Subject.Name})")
                .ToList(),
            AvailabilitySlots = instructor.InstructorAvailabilities
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartingTime)
                .Select(a => new InstructorAvailabilityViewModel
                {
                    Id = a.Id,
                    DayOfWeek = a.DayOfWeek,
                    StartingTime = a.StartingTime,
                    EndingTime = a.EndingTime
                })
                .ToList(),
            AssignedSessions = instructor.CourseSessions
                .OrderBy(s => s.StartingDate)
                .ThenBy(s => s.StartingTime)
                .Select(s => new InstructorAssignedSessionViewModel
                {
                    SessionId = s.Id,
                    CourseName = s.Course.Name,
                    ClassroomName = CourseSessionsController.FormatClassroom(s.Classroom),
                    StartingDate = s.StartingDate,
                    StartingTime = s.StartingTime,
                    EndingTime = s.EndingTime,
                    EnrolledCount = s.TraineeSessions.Count
                })
                .ToList()
        });
    }

    public async Task<IActionResult> EditQualifications(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var instructor = await FindInstructorAsync(id);
        if (instructor is null)
        {
            return NotFound();
        }

        return View(await BuildQualificationsModelAsync(instructor));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditQualifications(string id, InstructorQualificationsEditViewModel model)
    {
        if (id != model.InstructorId)
        {
            return NotFound();
        }

        var instructor = await FindInstructorAsync(id);
        if (instructor is null)
        {
            return NotFound();
        }

        var selectedCourseIds = model.Courses
            .Where(c => c.IsSelected)
            .Select(c => c.CourseId)
            .ToHashSet();

        var validCourseIds = await _context.Courses
            .Where(c => selectedCourseIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync();

        if (validCourseIds.Count != selectedCourseIds.Count)
        {
            ModelState.AddModelError(string.Empty, "One or more selected courses no longer exist.");
        }

        if (!ModelState.IsValid)
        {
            return View(await BuildQualificationsModelAsync(instructor, selectedCourseIds));
        }

        var existingQualifications = await _context.instructorQualifications
            .Where(q => q.InstructorId == id)
            .ToListAsync();

        _context.instructorQualifications.RemoveRange(existingQualifications);

        foreach (var courseId in selectedCourseIds)
        {
            _context.instructorQualifications.Add(new InstructorQualification
            {
                InstructorId = id,
                CourseId = courseId,
                Instructor = null!,
                Course = null!
            });
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Instructor expertise updated successfully.";

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Availability(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var instructor = await FindInstructorAsync(id);
        if (instructor is null)
        {
            return NotFound();
        }

        await PopulateAvailabilityViewDataAsync(instructor);
        return View(new InstructorAvailabilityFormViewModel { InstructorId = id });
    }

    public async Task<IActionResult> EditAvailability(string? id, int availabilityId)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var instructor = await FindInstructorAsync(id);
        if (instructor is null)
        {
            return NotFound();
        }

        var availability = await _context.InstructorAvailabilities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == availabilityId && a.InstructorId == id);

        if (availability is null)
        {
            return NotFound();
        }

        PopulateAvailabilityFormViewData(instructor);
        ViewBag.AvailabilityId = availabilityId;

        return View(new InstructorAvailabilityFormViewModel
        {
            InstructorId = id,
            DayOfWeek = availability.DayOfWeek,
            StartingTime = availability.StartingTime,
            EndingTime = availability.EndingTime
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAvailability(string id, InstructorAvailabilityFormViewModel model)
    {
        if (id != model.InstructorId)
        {
            return NotFound();
        }

        var instructor = await FindInstructorAsync(id);
        if (instructor is null)
        {
            return NotFound();
        }

        if (model.EndingTime <= model.StartingTime)
        {
            ModelState.AddModelError(nameof(model.EndingTime), "Ending time must be after starting time.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateAvailabilityViewDataAsync(instructor);
            return View(nameof(Availability), model);
        }

        _context.InstructorAvailabilities.Add(new InstructorAvailability
        {
            InstructorId = id,
            DayOfWeek = model.DayOfWeek,
            StartingTime = model.StartingTime,
            EndingTime = model.EndingTime
        });

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Availability slot added successfully.";

        return RedirectToAction(nameof(Availability), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAvailability(string id, int availabilityId, InstructorAvailabilityFormViewModel model)
    {
        if (id != model.InstructorId)
        {
            return NotFound();
        }

        var instructor = await FindInstructorAsync(id);
        if (instructor is null)
        {
            return NotFound();
        }

        var availability = await _context.InstructorAvailabilities
            .FirstOrDefaultAsync(a => a.Id == availabilityId && a.InstructorId == id);

        if (availability is null)
        {
            return NotFound();
        }

        if (model.EndingTime <= model.StartingTime)
        {
            ModelState.AddModelError(nameof(model.EndingTime), "Ending time must be after starting time.");
        }

        if (!ModelState.IsValid)
        {
            PopulateAvailabilityFormViewData(instructor);
            ViewBag.AvailabilityId = availabilityId;
            return View(model);
        }

        availability.DayOfWeek = model.DayOfWeek;
        availability.StartingTime = model.StartingTime;
        availability.EndingTime = model.EndingTime;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Availability slot updated successfully.";

        return RedirectToAction(nameof(Availability), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAvailability(string id, int availabilityId)
    {
        var availability = await _context.InstructorAvailabilities
            .FirstOrDefaultAsync(a => a.Id == availabilityId && a.InstructorId == id);

        if (availability is null)
        {
            return NotFound();
        }

        _context.InstructorAvailabilities.Remove(availability);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Availability slot removed successfully.";

        return RedirectToAction(nameof(Availability), new { id });
    }

    private async Task<User?> FindInstructorAsync(string id)
    {
        return await _context.Users
            .Include(u => u.UserInfo)
            .FirstOrDefaultAsync(u => u.Id == id && u.UserType == UserType.Instructor);
    }

    private async Task<InstructorQualificationsEditViewModel> BuildQualificationsModelAsync(
        User instructor,
        ISet<int>? selectedCourseIds = null)
    {
        selectedCourseIds ??= await _context.instructorQualifications
            .Where(q => q.InstructorId == instructor.Id)
            .Select(q => q.CourseId)
            .ToHashSetAsync();

        var courses = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Subject)
            .OrderBy(c => c.Subject.Name)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return new InstructorQualificationsEditViewModel
        {
            InstructorId = instructor.Id,
            InstructorName = FormatUser(instructor),
            Courses = courses.Select(c => new CourseQualificationOptionViewModel
            {
                CourseId = c.Id,
                CourseName = c.Name,
                SubjectName = c.Subject.Name,
                IsSelected = selectedCourseIds.Contains(c.Id)
            }).ToList()
        };
    }

    private async Task PopulateAvailabilityViewDataAsync(User instructor)
    {
        PopulateAvailabilityFormViewData(instructor);

        ViewBag.AvailabilitySlots = await _context.InstructorAvailabilities
            .AsNoTracking()
            .Where(a => a.InstructorId == instructor.Id)
            .OrderBy(a => a.DayOfWeek)
            .ThenBy(a => a.StartingTime)
            .Select(a => new InstructorAvailabilityViewModel
            {
                Id = a.Id,
                DayOfWeek = a.DayOfWeek,
                StartingTime = a.StartingTime,
                EndingTime = a.EndingTime
            })
            .ToListAsync();
    }

    private void PopulateAvailabilityFormViewData(User instructor)
    {
        ViewBag.InstructorName = FormatUser(instructor);
        ViewBag.DayOptions = Enum.GetValues<DayOfWeek>()
            .Select(day => new SelectListItem(day.ToString(), day.ToString()))
            .ToList();
    }

    private static string FormatUser(User user)
    {
        return user.UserInfo is null
            ? user.UserName ?? user.Email ?? user.Id
            : $"{user.UserInfo.FirstName} {user.UserInfo.LastName}";
    }
}

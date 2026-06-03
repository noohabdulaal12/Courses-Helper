using CoursesHelperMVC.Models;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

[Authorize(Roles = "Coordinator")]
public class CoursesController : Controller
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Subject)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Subject)
            .FirstOrDefaultAsync(c => c.Id == id);

        return course is null ? NotFound() : View(course);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateSubjectsAsync();
        return View(new CourseFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseFormViewModel model)
    {
        await ValidateSubjectAsync(model.SubjectId);

        if (!ModelState.IsValid)
        {
            await PopulateSubjectsAsync(model.SubjectId);
            return View(model);
        }

        var course = new Course
        {
            Name = model.Name,
            Description = model.Description,
            SubjectId = model.SubjectId,
            NumberOfSessions = model.NumberOfSessions,
            Price = model.Price
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Course created successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var course = await _context.Courses.FindAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        await PopulateSubjectsAsync(course.SubjectId);
        return View(ToFormModel(course));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        await ValidateSubjectAsync(model.SubjectId);

        if (!ModelState.IsValid)
        {
            await PopulateSubjectsAsync(model.SubjectId);
            return View(model);
        }

        var course = await _context.Courses.FindAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        course.Name = model.Name;
        course.Description = model.Description;
        course.SubjectId = model.SubjectId;
        course.NumberOfSessions = model.NumberOfSessions;
        course.Price = model.Price;

        try
        {
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Course updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CourseExistsAsync(id))
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

        var course = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Subject)
            .FirstOrDefaultAsync(c => c.Id == id);

        return course is null ? NotFound() : View(course);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        try
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Course deleted successfully.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "This course cannot be deleted because it is used by related training records.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSubjectsAsync(int? selectedSubjectId = null)
    {
        var subjects = await _context.Subjects
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();

        ViewBag.Subjects = new SelectList(subjects, nameof(Subject.Id), nameof(Subject.Name), selectedSubjectId);
    }

    private async Task ValidateSubjectAsync(int subjectId)
    {
        if (!await _context.Subjects.AnyAsync(s => s.Id == subjectId))
        {
            ModelState.AddModelError(nameof(CourseFormViewModel.SubjectId), "Please choose an existing subject.");
        }
    }

    private async Task<bool> CourseExistsAsync(int id)
    {
        return await _context.Courses.AnyAsync(e => e.Id == id);
    }

    private static CourseFormViewModel ToFormModel(Course course)
    {
        return new CourseFormViewModel
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            SubjectId = course.SubjectId,
            NumberOfSessions = course.NumberOfSessions,
            Price = course.Price
        };
    }
}

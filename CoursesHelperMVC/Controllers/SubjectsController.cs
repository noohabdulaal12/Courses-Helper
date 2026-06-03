using CoursesHelperMVC.Models;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

[Authorize(Roles = "Coordinator")]
public class SubjectsController : Controller
{
    private readonly AppDbContext _context;

    public SubjectsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var subjects = await _context.Subjects
            .AsNoTracking()
            .Include(s => s.Courses)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(subjects);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var subject = await _context.Subjects
            .AsNoTracking()
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.Id == id);

        return subject is null ? NotFound() : View(subject);
    }

    public IActionResult Create()
    {
        return View(new SubjectFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubjectFormViewModel model)
    {
        await ValidateUniqueNameAsync(model.Name);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var subject = new Subject
        {
            Name = model.Name,
            Description = model.Description
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Subject created successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var subject = await _context.Subjects.FindAsync(id);
        if (subject is null)
        {
            return NotFound();
        }

        return View(ToFormModel(subject));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubjectFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        await ValidateUniqueNameAsync(model.Name, id);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var subject = await _context.Subjects.FindAsync(id);
        if (subject is null)
        {
            return NotFound();
        }

        subject.Name = model.Name;
        subject.Description = model.Description;

        try
        {
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Subject updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await SubjectExistsAsync(id))
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

        var subject = await _context.Subjects
            .AsNoTracking()
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.Id == id);

        return subject is null ? NotFound() : View(subject);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var subject = await _context.Subjects
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subject is null)
        {
            return NotFound();
        }

        if (subject.Courses.Any())
        {
            TempData["ErrorMessage"] = "This subject cannot be deleted because it has related courses.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Subject deleted successfully.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "This subject cannot be deleted because it is used by related training records.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateUniqueNameAsync(string name, int? subjectId = null)
    {
        if (await _context.Subjects.AnyAsync(s => s.Name == name && s.Id != subjectId))
        {
            ModelState.AddModelError(nameof(SubjectFormViewModel.Name), "A subject with this name already exists.");
        }
    }

    private async Task<bool> SubjectExistsAsync(int id)
    {
        return await _context.Subjects.AnyAsync(s => s.Id == id);
    }

    private static SubjectFormViewModel ToFormModel(Subject subject)
    {
        return new SubjectFormViewModel
        {
            Id = subject.Id,
            Name = subject.Name,
            Description = subject.Description
        };
    }
}

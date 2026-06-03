using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class TraineeQualificationsController : Controller
    {
        private readonly AppDbContext _context;

        public TraineeQualificationsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var qualifications = _context.TraineeQualifications
                .Include(t => t.Course)
                .Include(t => t.Trainee);

            return View(await qualifications.ToListAsync());
        }

        public async Task<IActionResult> Details(string traineeId, int? courseId)
        {
            if (string.IsNullOrWhiteSpace(traineeId) || courseId == null)
            {
                TempData["ErrorMessage"] = "Choose a trainee qualification from the list to view its details.";
                return RedirectToAction(nameof(Index));
            }

            var traineeQualification = await _context.TraineeQualifications
                .Include(t => t.Course)
                .Include(t => t.Trainee)
                .FirstOrDefaultAsync(m => m.TraineeId == traineeId && m.CourseId == courseId);

            if (traineeQualification == null)
            {
                return NotFound();
            }

            return View(traineeQualification);
        }

        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TraineeId,CourseId")] TraineeQualification traineeQualification)
        {
            if (ModelState.IsValid)
            {
                var alreadyExists = await TraineeQualificationExistsAsync(traineeQualification.TraineeId, traineeQualification.CourseId);
                if (alreadyExists)
                {
                    ModelState.AddModelError(string.Empty, "This trainee already has this course qualification.");
                }
                else
                {
                    _context.Add(traineeQualification);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            PopulateSelectLists(traineeQualification.TraineeId, traineeQualification.CourseId);
            return View(traineeQualification);
        }

        public async Task<IActionResult> Edit(string traineeId, int? courseId)
        {
            if (string.IsNullOrWhiteSpace(traineeId) || courseId == null)
            {
                TempData["ErrorMessage"] = "Choose a trainee qualification from the list before editing.";
                return RedirectToAction(nameof(Index));
            }

            var traineeQualification = await _context.TraineeQualifications.FindAsync(traineeId, courseId.Value);
            if (traineeQualification == null)
            {
                return NotFound();
            }

            PopulateSelectLists(traineeQualification.TraineeId, traineeQualification.CourseId);
            return View(traineeQualification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string traineeId, int courseId, [Bind("TraineeId,CourseId")] TraineeQualification traineeQualification)
        {
            if (traineeId != traineeQualification.TraineeId || courseId != traineeQualification.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(traineeQualification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TraineeQualificationExistsAsync(traineeQualification.TraineeId, traineeQualification.CourseId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(traineeQualification.TraineeId, traineeQualification.CourseId);
            return View(traineeQualification);
        }

        public async Task<IActionResult> Delete(string traineeId, int? courseId)
        {
            if (string.IsNullOrWhiteSpace(traineeId) || courseId == null)
            {
                TempData["ErrorMessage"] = "Choose a trainee qualification from the list before deleting.";
                return RedirectToAction(nameof(Index));
            }

            var traineeQualification = await _context.TraineeQualifications
                .Include(t => t.Course)
                .Include(t => t.Trainee)
                .FirstOrDefaultAsync(m => m.TraineeId == traineeId && m.CourseId == courseId);

            if (traineeQualification == null)
            {
                return NotFound();
            }

            return View(traineeQualification);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string traineeId, int courseId)
        {
            var traineeQualification = await _context.TraineeQualifications.FindAsync(traineeId, courseId);
            if (traineeQualification != null)
            {
                _context.TraineeQualifications.Remove(traineeQualification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateSelectLists(string? selectedTraineeId = null, int? selectedCourseId = null)
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", selectedCourseId);
            ViewData["TraineeId"] = new SelectList(_context.Users.Where(u => u.UserType == UserType.Trainee), "Id", "Email", selectedTraineeId);
        }

        private Task<bool> TraineeQualificationExistsAsync(string traineeId, int courseId)
        {
            return _context.TraineeQualifications.AnyAsync(e => e.TraineeId == traineeId && e.CourseId == courseId);
        }
    }
}

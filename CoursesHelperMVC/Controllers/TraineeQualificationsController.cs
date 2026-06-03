using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: TraineeQualifications
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.TraineeQualifications.Include(t => t.Course).Include(t => t.Trainee);
            return View(await appDbContext.ToListAsync());
        }

        // GET: TraineeQualifications/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traineeQualification = await _context.TraineeQualifications
                .Include(t => t.Course)
                .Include(t => t.Trainee)
                .FirstOrDefaultAsync(m => m.TraineeId == id);
            if (traineeQualification == null)
            {
                return NotFound();
            }

            return View(traineeQualification);
        }

        // GET: TraineeQualifications/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            ViewData["TraineeId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: TraineeQualifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TraineeId,CourseId")] TraineeQualification traineeQualification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(traineeQualification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", traineeQualification.CourseId);
            ViewData["TraineeId"] = new SelectList(_context.Users, "Id", "Id", traineeQualification.TraineeId);
            return View(traineeQualification);
        }

        // GET: TraineeQualifications/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traineeQualification = await _context.TraineeQualifications.FindAsync(id);
            if (traineeQualification == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", traineeQualification.CourseId);
            ViewData["TraineeId"] = new SelectList(_context.Users, "Id", "Id", traineeQualification.TraineeId);
            return View(traineeQualification);
        }

        // POST: TraineeQualifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TraineeId,CourseId")] TraineeQualification traineeQualification)
        {
            if (id != traineeQualification.TraineeId)
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
                    if (!TraineeQualificationExists(traineeQualification.TraineeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", traineeQualification.CourseId);
            ViewData["TraineeId"] = new SelectList(_context.Users, "Id", "Id", traineeQualification.TraineeId);
            return View(traineeQualification);
        }

        // GET: TraineeQualifications/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traineeQualification = await _context.TraineeQualifications
                .Include(t => t.Course)
                .Include(t => t.Trainee)
                .FirstOrDefaultAsync(m => m.TraineeId == id);
            if (traineeQualification == null)
            {
                return NotFound();
            }

            return View(traineeQualification);
        }

        // POST: TraineeQualifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var traineeQualification = await _context.TraineeQualifications.FindAsync(id);
            if (traineeQualification != null)
            {
                _context.TraineeQualifications.Remove(traineeQualification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TraineeQualificationExists(string id)
        {
            return _context.TraineeQualifications.Any(e => e.TraineeId == id);
        }
    }
}

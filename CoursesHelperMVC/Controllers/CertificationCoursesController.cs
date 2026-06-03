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
    public class CertificationCoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CertificationCoursesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CertificationCourses
        public async Task<IActionResult> Index(int? certificationId)
        {
            var certificationCourses = _context.CertificationCourses
                .Include(c => c.Certification)
                .Include(c => c.Course)
                .AsQueryable();

            if (certificationId.HasValue)
            {
                certificationCourses = certificationCourses.Where(c => c.CertificationId == certificationId.Value);
                ViewData["CertificationId"] = certificationId.Value;
                ViewData["CertificationName"] = await _context.Certifications
                    .Where(c => c.Id == certificationId.Value)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
            }

            return View(await certificationCourses.ToListAsync());
        }

        // GET: CertificationCourses/Details/5
        public async Task<IActionResult> Details(int? certificationId, int? courseId)
        {
            if (certificationId == null || courseId == null)
            {
                return NotFound();
            }

            var certificationCourse = await _context.CertificationCourses
                .Include(c => c.Certification)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.CertificationId == certificationId && m.CourseId == courseId);
            if (certificationCourse == null)
            {
                return NotFound();
            }

            return View(certificationCourse);
        }

        // GET: CertificationCourses/Create
        public IActionResult Create(int? certificationId)
        {
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", certificationId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            return View(new CertificationCourse { CertificationId = certificationId ?? 0, CourseId = 0 });
        }

        // POST: CertificationCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CertificationId,CourseId")] CertificationCourse certificationCourse)
        {
            if (ModelState.IsValid)
            {
                var alreadyExists = await CertificationCourseExistsAsync(certificationCourse.CertificationId, certificationCourse.CourseId);
                if (alreadyExists)
                {
                    ModelState.AddModelError(string.Empty, "This course is already assigned to this certification.");
                }
                else
                {
                    _context.Add(certificationCourse);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { certificationId = certificationCourse.CertificationId });
                }
            }
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", certificationCourse.CertificationId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", certificationCourse.CourseId);
            return View(certificationCourse);
        }

        // GET: CertificationCourses/Edit/5
        public async Task<IActionResult> Edit(int? certificationId, int? courseId)
        {
            if (certificationId == null || courseId == null)
            {
                return NotFound();
            }

            var certificationCourse = await _context.CertificationCourses.FindAsync(certificationId.Value, courseId.Value);
            if (certificationCourse == null)
            {
                return NotFound();
            }
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", certificationCourse.CertificationId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", certificationCourse.CourseId);
            return View(certificationCourse);
        }

        // POST: CertificationCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int certificationId, int courseId, [Bind("CertificationId,CourseId")] CertificationCourse certificationCourse)
        {
            if (certificationId != certificationCourse.CertificationId || courseId != certificationCourse.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(certificationCourse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CertificationCourseExistsAsync(certificationCourse.CertificationId, certificationCourse.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { certificationId = certificationCourse.CertificationId });
            }
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", certificationCourse.CertificationId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", certificationCourse.CourseId);
            return View(certificationCourse);
        }

        // GET: CertificationCourses/Delete/5
        public async Task<IActionResult> Delete(int? certificationId, int? courseId)
        {
            if (certificationId == null || courseId == null)
            {
                return NotFound();
            }

            var certificationCourse = await _context.CertificationCourses
                .Include(c => c.Certification)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.CertificationId == certificationId && m.CourseId == courseId);
            if (certificationCourse == null)
            {
                return NotFound();
            }

            return View(certificationCourse);
        }

        // POST: CertificationCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int certificationId, int courseId)
        {
            var certificationCourse = await _context.CertificationCourses.FindAsync(certificationId, courseId);
            if (certificationCourse != null)
            {
                _context.CertificationCourses.Remove(certificationCourse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { certificationId });
        }

        private Task<bool> CertificationCourseExistsAsync(int certificationId, int courseId)
        {
            return _context.CertificationCourses.AnyAsync(e => e.CertificationId == certificationId && e.CourseId == courseId);
        }
    }
}

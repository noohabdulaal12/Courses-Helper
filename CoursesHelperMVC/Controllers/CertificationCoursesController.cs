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
    [Authorize(Roles = "Admin, Coordinator")]
    public class CertificationCoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CertificationCoursesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CertificationCourses
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CertificationCourses.Include(c => c.Certification).Include(c => c.Course);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CertificationCourses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificationCourse = await _context.CertificationCourses
                .Include(c => c.Certification)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.CertificationId == id);
            if (certificationCourse == null)
            {
                return NotFound();
            }

            return View(certificationCourse);
        }

        // GET: CertificationCourses/Create
        public IActionResult Create()
        {
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name");
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        // POST: CertificationCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CertificationId,CourseId")] CertificationCourse certificationCourse)
        {
            bool alreadyExists = await _context.CertificationCourses.AnyAsync(cc => cc.CertificationId == certificationCourse.CertificationId
            && cc.CourseId == certificationCourse.CourseId);
            
            if (ModelState.IsValid)
            {

                if (!alreadyExists)
                {
                    _context.Add(certificationCourse);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This course is already assigned to this certification.");
                }

                await _context.SaveChangesAsync();
            }
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", certificationCourse.CertificationId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", certificationCourse.CourseId);
            return View(certificationCourse);
        }

        // GET: CertificationCourses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificationCourse = await _context.CertificationCourses.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("CertificationId,CourseId")] CertificationCourse certificationCourse)
        {
            if (id != certificationCourse.CertificationId)
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
                    if (!CertificationCourseExists(certificationCourse.CertificationId))
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
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", certificationCourse.CertificationId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", certificationCourse.CourseId);
            return View(certificationCourse);
        }

        // GET: CertificationCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificationCourse = await _context.CertificationCourses
                .Include(c => c.Certification)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.CertificationId == id);
            if (certificationCourse == null)
            {
                return NotFound();
            }

            return View(certificationCourse);
        }

        // POST: CertificationCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificationCourse = await _context.CertificationCourses.FindAsync(id);
            if (certificationCourse != null)
            {
                _context.CertificationCourses.Remove(certificationCourse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificationCourseExists(int id)
        {
            return _context.CertificationCourses.Any(e => e.CertificationId == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;

namespace CoursesHelperMVC.Controllers
{
    public class TraineeCertificationsController : Controller
    {
        private readonly AppDbContext _context;

        public TraineeCertificationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TraineeCertifications
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.TraineeCertifications.Include(t => t.Certification).Include(t => t.Trainee);
            return View(await appDbContext.ToListAsync());
        }

        // GET: TraineeCertifications/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traineeCertification = await _context.TraineeCertifications
                .Include(t => t.Certification)
                .Include(t => t.Trainee)
                .FirstOrDefaultAsync(m => m.TraineeId == id);
            if (traineeCertification == null)
            {
                return NotFound();
            }

            return View(traineeCertification);
        }

        // GET: TraineeCertifications/Create
        public IActionResult Create()
        {
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name");
            ViewData["TraineeId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: TraineeCertifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TraineeId,CertificationId")] TraineeCertification traineeCertification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(traineeCertification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", traineeCertification.CertificationId);
            ViewData["TraineeId"] = new SelectList(_context.Users, "Id", "Id", traineeCertification.TraineeId);
            return View(traineeCertification);
        }

        // GET: TraineeCertifications/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traineeCertification = await _context.TraineeCertifications.FindAsync(id);
            if (traineeCertification == null)
            {
                return NotFound();
            }
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", traineeCertification.CertificationId);
            ViewData["TraineeId"] = new SelectList(_context.Users, "Id", "Id", traineeCertification.TraineeId);
            return View(traineeCertification);
        }

        // POST: TraineeCertifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TraineeId,CertificationId")] TraineeCertification traineeCertification)
        {
            if (id != traineeCertification.TraineeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(traineeCertification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TraineeCertificationExists(traineeCertification.TraineeId))
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
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", traineeCertification.CertificationId);
            ViewData["TraineeId"] = new SelectList(_context.Users, "Id", "Id", traineeCertification.TraineeId);
            return View(traineeCertification);
        }

        // GET: TraineeCertifications/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traineeCertification = await _context.TraineeCertifications
                .Include(t => t.Certification)
                .Include(t => t.Trainee)
                .FirstOrDefaultAsync(m => m.TraineeId == id);
            if (traineeCertification == null)
            {
                return NotFound();
            }

            return View(traineeCertification);
        }

        // POST: TraineeCertifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var traineeCertification = await _context.TraineeCertifications.FindAsync(id);
            if (traineeCertification != null)
            {
                _context.TraineeCertifications.Remove(traineeCertification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TraineeCertificationExists(string id)
        {
            return _context.TraineeCertifications.Any(e => e.TraineeId == id);
        }
    }
}

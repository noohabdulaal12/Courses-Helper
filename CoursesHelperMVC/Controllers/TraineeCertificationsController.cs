using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoursesHelperMVC.Models;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CoursesHelperMVC.Controllers
{
    [Authorize(Roles = "Coordinator")]
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

        public async Task<IActionResult> Progress(string? traineeId, int? certificationId, string status = "All")
        {
            var assignmentsQuery = _context.TraineeCertifications
                .Include(tc => tc.Trainee)
                .ThenInclude(t => t.UserInfo)
                .Include(tc => tc.Certification)
                .ThenInclude(c => c.CertificationCourses)
                .ThenInclude(cc => cc.Course)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(traineeId))
            {
                assignmentsQuery = assignmentsQuery.Where(tc => tc.TraineeId == traineeId);
            }

            if (certificationId.HasValue)
            {
                assignmentsQuery = assignmentsQuery.Where(tc => tc.CertificationId == certificationId.Value);
            }

            var assignments = await assignmentsQuery
                .OrderBy(tc => tc.Trainee.Email)
                .ThenBy(tc => tc.Certification.Name)
                .ToListAsync();

            var selectedTraineeIds = assignments.Select(a => a.TraineeId).Distinct().ToList();
            var qualifications = await _context.TraineeQualifications
                .Include(tq => tq.Course)
                .AsNoTracking()
                .Where(tq => selectedTraineeIds.Contains(tq.TraineeId))
                .ToListAsync();

            var qualificationsByTrainee = qualifications
                .GroupBy(q => q.TraineeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var rows = assignments
                .Select(assignment =>
                {
                    var requiredCourses = assignment.Certification.CertificationCourses
                        .Where(cc => cc.Course is not null)
                        .OrderBy(cc => cc.Course!.Name)
                        .ToList();

                    qualificationsByTrainee.TryGetValue(assignment.TraineeId, out var traineeQualifications);
                    traineeQualifications ??= [];

                    var completedCourseIds = traineeQualifications
                        .Select(q => q.CourseId)
                        .ToHashSet();

                    var requiredCourseIds = requiredCourses
                        .Select(cc => cc.CourseId)
                        .ToHashSet();

                    var completedRequiredCourses = requiredCourses
                        .Where(cc => completedCourseIds.Contains(cc.CourseId))
                        .Select(cc => cc.Course!.Name)
                        .ToList();

                    var missingCourses = requiredCourses
                        .Where(cc => !completedCourseIds.Contains(cc.CourseId))
                        .Select(cc => cc.Course!.Name)
                        .ToList();

                    var userInfo = assignment.Trainee.UserInfo;
                    var traineeName = userInfo is null
                        ? assignment.Trainee.Email ?? assignment.Trainee.UserName ?? assignment.TraineeId
                        : $"{userInfo.FirstName} {userInfo.LastName}";

                    return new CertificationProgressRowViewModel
                    {
                        TraineeId = assignment.TraineeId,
                        CertificationId = assignment.CertificationId,
                        TraineeName = traineeName,
                        TraineeEmail = assignment.Trainee.Email ?? string.Empty,
                        CertificationName = assignment.Certification.Name,
                        RequiredCourses = requiredCourses.Select(cc => cc.Course!.Name).ToList(),
                        CompletedCourses = completedRequiredCourses,
                        MissingCourses = missingCourses
                    };
                })
                .ToList();

            rows = status switch
            {
                "Complete" => rows.Where(row => row.IsComplete).ToList(),
                "Incomplete" => rows.Where(row => !row.IsComplete).ToList(),
                _ => rows
            };

            var trainees = await _context.Users
                .Include(u => u.UserInfo)
                .AsNoTracking()
                .Where(u => u.UserType == UserType.Trainee)
                .OrderBy(u => u.Email)
                .ToListAsync();

            var certifications = await _context.Certifications
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            var model = new CertificationProgressIndexViewModel
            {
                TraineeId = traineeId,
                CertificationId = certificationId,
                Status = status,
                TraineeOptions = trainees.Select(t =>
                {
                    var name = t.UserInfo is null ? t.Email ?? t.UserName ?? t.Id : $"{t.UserInfo.FirstName} {t.UserInfo.LastName} ({t.Email})";
                    return new SelectListItem(name, t.Id, t.Id == traineeId);
                }).ToList(),
                CertificationOptions = certifications.Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == certificationId)).ToList(),
                Rows = rows
            };

            return View(model);
        }

        // GET: TraineeCertifications/Details/5
        public async Task<IActionResult> Details(string traineeId, int? certificationId)
        {
            if (string.IsNullOrWhiteSpace(traineeId) || certificationId == null)
            {
                return NotFound();
            }

            var traineeCertification = await _context.TraineeCertifications
                .Include(t => t.Certification)
                .Include(t => t.Trainee)
                .FirstOrDefaultAsync(m => m.TraineeId == traineeId && m.CertificationId == certificationId);
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
            ViewData["TraineeId"] = new SelectList(_context.Users.Where(u => u.UserType == UserType.Trainee), "Id", "Email");
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
                var alreadyExists = await TraineeCertificationExistsAsync(traineeCertification.TraineeId, traineeCertification.CertificationId);
                if (alreadyExists)
                {
                    ModelState.AddModelError(string.Empty, "This trainee is already assigned to this certification.");
                }
                else
                {
                    _context.Add(traineeCertification);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", traineeCertification.CertificationId);
            ViewData["TraineeId"] = new SelectList(_context.Users.Where(u => u.UserType == UserType.Trainee), "Id", "Email", traineeCertification.TraineeId);
            return View(traineeCertification);
        }

        // GET: TraineeCertifications/Edit/5
        public async Task<IActionResult> Edit(string traineeId, int? certificationId)
        {
            if (string.IsNullOrWhiteSpace(traineeId) || certificationId == null)
            {
                return NotFound();
            }

            var traineeCertification = await _context.TraineeCertifications.FindAsync(traineeId, certificationId.Value);
            if (traineeCertification == null)
            {
                return NotFound();
            }
            ViewData["CertificationId"] = new SelectList(_context.Certifications, "Id", "Name", traineeCertification.CertificationId);
            ViewData["TraineeId"] = new SelectList(_context.Users.Where(u => u.UserType == UserType.Trainee), "Id", "Email", traineeCertification.TraineeId);
            return View(traineeCertification);
        }

        // POST: TraineeCertifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string traineeId, int certificationId, [Bind("TraineeId,CertificationId")] TraineeCertification traineeCertification)
        {
            if (traineeId != traineeCertification.TraineeId || certificationId != traineeCertification.CertificationId)
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
                    if (!await TraineeCertificationExistsAsync(traineeCertification.TraineeId, traineeCertification.CertificationId))
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
            ViewData["TraineeId"] = new SelectList(_context.Users.Where(u => u.UserType == UserType.Trainee), "Id", "Email", traineeCertification.TraineeId);
            return View(traineeCertification);
        }

        // GET: TraineeCertifications/Delete/5
        public async Task<IActionResult> Delete(string traineeId, int? certificationId)
        {
            if (string.IsNullOrWhiteSpace(traineeId) || certificationId == null)
            {
                return NotFound();
            }

            var traineeCertification = await _context.TraineeCertifications
                .Include(t => t.Certification)
                .Include(t => t.Trainee)
                .FirstOrDefaultAsync(m => m.TraineeId == traineeId && m.CertificationId == certificationId);
            if (traineeCertification == null)
            {
                return NotFound();
            }

            return View(traineeCertification);
        }

        // POST: TraineeCertifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string traineeId, int certificationId)
        {
            var traineeCertification = await _context.TraineeCertifications.FindAsync(traineeId, certificationId);
            if (traineeCertification != null)
            {
                _context.TraineeCertifications.Remove(traineeCertification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> TraineeCertificationExistsAsync(string traineeId, int certificationId)
        {
            return _context.TraineeCertifications.AnyAsync(e => e.TraineeId == traineeId && e.CertificationId == certificationId);
        }
    }
}

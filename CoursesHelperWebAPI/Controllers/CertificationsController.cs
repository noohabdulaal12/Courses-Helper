using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.DTOs;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CoursesHelperWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CertificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Certifications
        [HttpGet]
        public async Task<ActionResult> GetCertifications()
        {
            var certifications = await _context.Certifications
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description
                })
                .ToListAsync();

            return Ok(certifications);
        }

        // GET: api/Certifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCertification(int id)
        {
            var certification = await _context.Certifications
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description
                })
                .FirstOrDefaultAsync();

            if (certification == null)
                return NotFound();

            return Ok(certification);
        }

        // POST: api/Certifications
        [HttpPost]
        public async Task<ActionResult> CreateCertification(CreateCertificationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            var certification = new Certification
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCertification), new { id = certification.Id }, new
            {
                certification.Id,
                certification.Name,
                certification.Description
            });
        }

        // PUT: api/Certifications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCertification(int id, CreateCertificationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            var existing = await _context.Certifications.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Certifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertification(int id)
        {
            var certification = await _context.Certifications.FindAsync(id);

            if (certification == null)
                return NotFound();

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Certifications/5/courses
        [HttpGet("{id}/courses")]
        public async Task<ActionResult> GetCertificationCourses(int id)
        {
            var certificationExists = await _context.Certifications.AnyAsync(c => c.Id == id);
            if (!certificationExists)
                return NotFound();

            var courses = await _context.CertificationCourses
                .Where(cc => cc.CertificationId == id)
                .Include(cc => cc.Course)
                .Select(cc => new
                {
                    cc.CourseId,
                    cc.Course.Name,
                    cc.Course.Description
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/Certifications/progress/{traineeId}/{certificationId}
        [HttpGet("progress/{traineeId}/{certificationId}")]
        public async Task<ActionResult> GetTraineeProgress(string traineeId, int certificationId)
        {
            var certification = await _context.Certifications.FindAsync(certificationId);
            if (certification == null)
                return NotFound("Certification not found.");

            var traineeExists = await _context.Users.AnyAsync(u => u.Id == traineeId);
            if (!traineeExists)
                return NotFound("Trainee not found.");

            var requiredCourses = await _context.CertificationCourses
                .Where(cc => cc.CertificationId == certificationId)
                .Include(cc => cc.Course)
                .Select(cc => new
                {
                    cc.CourseId,
                    cc.Course.Name
                })
                .ToListAsync();

            var completedCourseIds = await _context.TraineeSessions
                .Where(ts =>
                    ts.TraineeId == traineeId &&
                    ts.Status == Status.Completed)
                .Select(ts => ts.CourseSession.CourseId)
                .Distinct()
                .ToListAsync();

            var completedCourses = requiredCourses
                .Where(rc => completedCourseIds.Contains(rc.CourseId))
                .Select(rc => rc.Name)
                .ToList();

            var pendingCourses = requiredCourses
                .Where(rc => !completedCourseIds.Contains(rc.CourseId))
                .Select(rc => rc.Name)
                .ToList();

            return Ok(new
            {
                TraineeId = traineeId,
                CertificationId = certificationId,
                CertificationName = certification.Name,
                Completed = completedCourses.Count,
                Total = requiredCourses.Count,
                CompletedCourses = completedCourses,
                PendingCourses = pendingCourses,
                IsEligible = pendingCourses.Count == 0
            });
        }
    }
}

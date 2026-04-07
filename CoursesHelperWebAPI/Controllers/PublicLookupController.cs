using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.DTOs;
using CoursesHelperWebAPI.Models.Enums;

namespace CoursesHelperWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicLookupController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PublicLookupController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PublicLookup?traineeId=trainee-user2&certificationId=2
        [HttpGet]
        public async Task<ActionResult> Lookup([FromQuery] string traineeId, [FromQuery] int certificationId)
        {
            if (string.IsNullOrWhiteSpace(traineeId))
                return BadRequest("TraineeId is required.");

            var traineeCertification = await _context.TraineeCertifications
                .Include(tc => tc.Trainee)
                .Include(tc => tc.Certification)
                .FirstOrDefaultAsync(tc =>
                    tc.TraineeId == traineeId &&
                    tc.CertificationId == certificationId);

            if (traineeCertification == null)
                return NotFound("No matching certification record found.");

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

            var result = new PublicLookupResultDto
            {
                TraineeId = traineeCertification.TraineeId,
                TraineeName = traineeCertification.Trainee.UserName ?? string.Empty,
                CertificationId = traineeCertification.CertificationId,
                CertificationName = traineeCertification.Certification.Name,
                TotalRequiredCourses = requiredCourses.Count,
                CompletedCount = completedCourses.Count,
                CompletedCourses = completedCourses,
                PendingCourses = pendingCourses,
                IsEligible = pendingCourses.Count == 0
            };

            return Ok(result);
        }
    }
}
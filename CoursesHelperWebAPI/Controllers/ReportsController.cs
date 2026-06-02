using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CoursesHelperWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Coordinator,Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Reports/enrollments-by-course
        [HttpGet("enrollments-by-course")]
        public async Task<ActionResult> GetEnrollmentsByCourse()
        {
            var report = await _context.TraineeSessions
                .Include(ts => ts.CourseSession)
                    .ThenInclude(cs => cs.Course)
                .GroupBy(ts => new
                {
                    ts.CourseSession.CourseId,
                    ts.CourseSession.Course.Name
                })
                .Select(g => new
                {
                    CourseId = g.Key.CourseId,
                    CourseName = g.Key.Name,
                    TotalEnrollments = g.Count(),
                    Completed = g.Count(x => x.Status == Status.Completed),
                    Requested = g.Count(x => x.Status == Status.Requested)
                })
                .OrderBy(x => x.CourseName)
                .ToListAsync();

            return Ok(report);
        }

        // GET: api/Reports/enrollments-by-subject
        [HttpGet("enrollments-by-subject")]
        public async Task<ActionResult> GetEnrollmentsBySubject()
        {
            var report = await _context.TraineeSessions
                .Include(ts => ts.CourseSession)
                    .ThenInclude(cs => cs.Course)
                        .ThenInclude(c => c.Subject)
                .GroupBy(ts => new
                {
                    ts.CourseSession.Course.SubjectId,
                    ts.CourseSession.Course.Subject.Name
                })
                .Select(g => new
                {
                    SubjectId = g.Key.SubjectId,
                    SubjectName = g.Key.Name,
                    TotalEnrollments = g.Count()
                })
                .OrderBy(x => x.SubjectName)
                .ToListAsync();

            return Ok(report);
        }

        // GET: api/Reports/instructor-workload
        [HttpGet("instructor-workload")]
        public async Task<ActionResult> GetInstructorWorkload()
        {
            var report = await _context.CourseSessions
                .Include(cs => cs.Instructor)
                .Include(cs => cs.TraineeSessions)
                .GroupBy(cs => new
                {
                    cs.InstructorId,
                    cs.Instructor.UserName
                })
                .Select(g => new
                {
                    InstructorId = g.Key.InstructorId,
                    InstructorName = g.Key.UserName,
                    TotalSessions = g.Count(),
                    TotalAssignedSeats = g.Sum(x => x.MaxSeats),
                    TotalEnrollments = g.Sum(x => x.TraineeSessions.Count)
                })
                .OrderBy(x => x.InstructorName)
                .ToListAsync();

            return Ok(report);
        }

        // GET: api/Reports/revenue-summary
        [HttpGet("revenue-summary")]
        public async Task<ActionResult> GetRevenueSummary()
        {
            var report = await _context.TraineeSessions
                .Include(ts => ts.CourseSession)
                    .ThenInclude(cs => cs.Course)
                .GroupBy(ts => new
                {
                    ts.CourseSession.CourseId,
                    ts.CourseSession.Course.Name
                })
                .Select(g => new
                {
                    CourseId = g.Key.CourseId,
                    CourseName = g.Key.Name,
                    EnrollmentCount = g.Count(),
                    ExpectedRevenue = g.Sum(x => x.CourseSession.Course.Price),
                    CollectedRevenue = g.Sum(x => x.AmountPaid),
                    OutstandingRevenue = g.Sum(x => x.CourseSession.Course.Price - x.AmountPaid)
                })
                .OrderBy(x => x.CourseName)
                .ToListAsync();

            return Ok(report);
        }

        // GET: api/Reports/certification-completion
        [HttpGet("certification-completion")]
        public async Task<ActionResult> GetCertificationCompletion()
        {
            var certifications = await _context.Certifications
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToListAsync();

            var result = new List<object>();

            foreach (var cert in certifications)
            {
                var requiredCourseIds = await _context.CertificationCourses
                    .Where(cc => cc.CertificationId == cert.Id)
                    .Select(cc => cc.CourseId)
                    .ToListAsync();

                var traineeIds = await _context.TraineeCertifications
                    .Where(tc => tc.CertificationId == cert.Id)
                    .Select(tc => tc.TraineeId)
                    .Distinct()
                    .ToListAsync();

                int eligibleCount = 0;

                foreach (var traineeId in traineeIds)
                {
                    var completedCourseIds = await _context.TraineeSessions
                        .Where(ts => ts.TraineeId == traineeId && ts.Status == Status.Completed)
                        .Select(ts => ts.CourseSession.CourseId)
                        .Distinct()
                        .ToListAsync();

                    bool eligible = requiredCourseIds.All(rc => completedCourseIds.Contains(rc));
                    if (eligible)
                        eligibleCount++;
                }

                result.Add(new
                {
                    CertificationId = cert.Id,
                    CertificationName = cert.Name,
                    TotalTrackedTrainees = traineeIds.Count,
                    EligibleTrainees = eligibleCount,
                    CompletionRate = traineeIds.Count == 0
                        ? 0
                        : Math.Round((double)eligibleCount / traineeIds.Count * 100, 2)
                });
            }

            return Ok(result);
        }
    }
}

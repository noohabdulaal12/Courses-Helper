using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using CoursesHelperWebAPI.DTOs;

namespace CoursesHelperWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Enrollments
        [HttpGet]
        public async Task<ActionResult> GetEnrollments()
        {
            var enrollments = await _context.TraineeSessions
                .Include(e => e.Trainee)
                .Include(e => e.CourseSession)
                    .ThenInclude(s => s.Course)
                .Select(e => new
                {
                    e.TraineeId,
                    TraineeName = e.Trainee.UserName,
                    e.SessionId,
                    CourseName = e.CourseSession.Course.Name,
                    e.AmountPaid,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // POST: api/Enrollments
        [HttpPost]
        public async Task<ActionResult> Enroll(CreateEnrollmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TraineeId))
                return BadRequest("TraineeId is required.");

            if (dto.AmountPaid < 0)
                return BadRequest("AmountPaid cannot be negative.");

            var trainee = await _context.Users.FindAsync(dto.TraineeId);
            if (trainee == null)
                return BadRequest("Trainee does not exist.");

            var session = await _context.CourseSessions
                .Include(s => s.TraineeSessions)
                .FirstOrDefaultAsync(s => s.Id == dto.SessionId);

            if (session == null)
                return BadRequest("Session does not exist.");

            bool alreadyEnrolled = await _context.TraineeSessions.AnyAsync(e =>
                e.TraineeId == dto.TraineeId &&
                e.SessionId == dto.SessionId);

            if (alreadyEnrolled)
                return BadRequest("Trainee already enrolled in this session.");

            int currentCount = session.TraineeSessions.Count;
            if (currentCount >= session.MaxSeats)
                return BadRequest("Session is full.");

            var traineeSessions = await _context.TraineeSessions
                .Include(e => e.CourseSession)
                .Where(e => e.TraineeId == dto.TraineeId)
                .ToListAsync();

            bool timeConflict = traineeSessions.Any(e =>
                e.CourseSession.StartingDate == session.StartingDate &&
                session.StartingTime < e.CourseSession.EndingTime &&
                session.EndingTime > e.CourseSession.StartingTime);

            if (timeConflict)
                return BadRequest("Trainee has a time conflict.");

            var enrollment = new TraineeSession
            {
                TraineeId = dto.TraineeId,
                SessionId = dto.SessionId,
                AmountPaid = dto.AmountPaid,
                Status = Status.Requested
            };

            _context.TraineeSessions.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                enrollment.TraineeId,
                enrollment.SessionId,
                enrollment.AmountPaid,
                enrollment.Status
            });
        }

        // PUT: api/Enrollments
        [HttpPut]
        public async Task<IActionResult> UpdateEnrollment(UpdateEnrollmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TraineeId))
                return BadRequest("TraineeId is required.");

            if (dto.AmountPaid < 0)
                return BadRequest("AmountPaid cannot be negative.");

            var existing = await _context.TraineeSessions
                .FirstOrDefaultAsync(e =>
                    e.TraineeId == dto.TraineeId &&
                    e.SessionId == dto.SessionId);

            if (existing == null)
                return NotFound();

            existing.AmountPaid = dto.AmountPaid;
            existing.Status = dto.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Enrollments?traineeId=...&sessionId=...
        [HttpDelete]
        public async Task<IActionResult> DeleteEnrollment(string traineeId, int sessionId)
        {
            var enrollment = await _context.TraineeSessions
                .FirstOrDefaultAsync(e =>
                    e.TraineeId == traineeId &&
                    e.SessionId == sessionId);

            if (enrollment == null)
                return NotFound();

            _context.TraineeSessions.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
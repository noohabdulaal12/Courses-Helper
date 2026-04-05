using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;

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
        public async Task<ActionResult> Enroll(TraineeSession enrollment)
        {
            // 1. Validate trainee
            var trainee = await _context.Users.FindAsync(enrollment.TraineeId);
            if (trainee == null)
                return BadRequest("Trainee does not exist.");

            // 2. Validate session
            var session = await _context.CourseSessions
                .Include(s => s.TraineeSessions)
                .FirstOrDefaultAsync(s => s.Id == enrollment.SessionId);

            if (session == null)
                return BadRequest("Session does not exist.");

            // 3. Prevent duplicate enrollment
            bool alreadyEnrolled = await _context.TraineeSessions.AnyAsync(e =>
                e.TraineeId == enrollment.TraineeId &&
                e.SessionId == enrollment.SessionId);

            if (alreadyEnrolled)
                return BadRequest("Trainee already enrolled in this session.");

            // 4. Capacity check
            int currentCount = session.TraineeSessions.Count;

            if (currentCount >= session.MaxSeats)
                return BadRequest("Session is full.");

            // 5. Time conflict check
            var traineeSessions = await _context.TraineeSessions
                .Include(e => e.CourseSession)
                .Where(e => e.TraineeId == enrollment.TraineeId)
                .ToListAsync();

            bool timeConflict = traineeSessions.Any(e =>
                e.CourseSession.StartingDate == session.StartingDate &&
                session.StartingTime < e.CourseSession.EndingTime &&
                session.EndingTime > e.CourseSession.StartingTime);

            if (timeConflict)
                return BadRequest("Trainee has a time conflict.");

            // SAVE
            enrollment.Status = Status.Requested;
            _context.TraineeSessions.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok("Enrollment successful.");
        }

        // PUT: api/Enrollments (update status/payment)
        [HttpPut]
        public async Task<IActionResult> UpdateEnrollment(TraineeSession updated)
        {
            var existing = await _context.TraineeSessions
                .FirstOrDefaultAsync(e =>
                    e.TraineeId == updated.TraineeId &&
                    e.SessionId == updated.SessionId);

            if (existing == null)
                return NotFound();

            existing.AmountPaid = updated.AmountPaid;
            existing.Status = updated.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Enrollments
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
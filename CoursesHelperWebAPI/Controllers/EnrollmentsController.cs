using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using CoursesHelperWebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using CoursesHelperWebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoursesHelperWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<EnrollmentHub> _hubContext;

        public EnrollmentsController(AppDbContext context, IHubContext<EnrollmentHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
                    CoursePrice = e.CourseSession.Course.Price,
                    e.AmountPaid,
                    e.PaymentDueDate,
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
                .Include(s => s.Course)
                .Include(s => s.TraineeSessions)
                .FirstOrDefaultAsync(s => s.Id == dto.SessionId);

            if (session == null)
                return BadRequest("Session does not exist.");

            if (dto.AmountPaid > session.Course.Price)
                return BadRequest("AmountPaid cannot exceed the course price.");

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
                PaymentDueDate = dto.PaymentDueDate,
                Status = Status.Requested
            };

            _context.TraineeSessions.Add(enrollment);
            await _context.SaveChangesAsync();

            await SendSeatUpdateAsync(dto.SessionId);

            return Ok(new
            {
                enrollment.TraineeId,
                enrollment.SessionId,
                enrollment.AmountPaid,
                enrollment.PaymentDueDate,
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

            if (!Enum.IsDefined(typeof(Status), dto.Status))
                return BadRequest("Status is invalid.");

            var existing = await _context.TraineeSessions
                .Include(e => e.CourseSession)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(e =>
                    e.TraineeId == dto.TraineeId &&
                    e.SessionId == dto.SessionId);

            if (existing == null)
                return NotFound();

            if (dto.AmountPaid > existing.CourseSession.Course.Price)
                return BadRequest("AmountPaid cannot exceed the course price.");

            var previousStatus = existing.Status;
            existing.AmountPaid = dto.AmountPaid;
            existing.PaymentDueDate = dto.PaymentDueDate;
            existing.Status = dto.Status;

            await SyncTraineeQualificationAsync(existing, previousStatus);

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

            await SendSeatUpdateAsync(sessionId);

            return NoContent();
        }

        private async Task SendSeatUpdateAsync(int sessionId)
        {
            var sessionSeats = await _context.CourseSessions
                .Where(s => s.Id == sessionId)
                .Select(s => new
                {
                    s.MaxSeats,
                    Enrolled = s.TraineeSessions.Count
                })
                .FirstOrDefaultAsync();

            if (sessionSeats == null)
                return;

            var remainingSeats = sessionSeats.MaxSeats - sessionSeats.Enrolled;

            await _hubContext.Clients.All.SendAsync(
                "ReceiveSeatUpdate",
                sessionId,
                remainingSeats);
        }

        private async Task SyncTraineeQualificationAsync(TraineeSession enrollment, Status previousStatus)
        {
            var courseId = await _context.CourseSessions
                .Where(s => s.Id == enrollment.SessionId)
                .Select(s => s.CourseId)
                .FirstAsync();

            if (enrollment.Status == Status.Completed)
            {
                var exists = await _context.TraineeQualifications.AnyAsync(q =>
                    q.TraineeId == enrollment.TraineeId &&
                    q.CourseId == courseId);

                if (!exists)
                {
                    _context.TraineeQualifications.Add(new TraineeQualification
                    {
                        TraineeId = enrollment.TraineeId,
                        CourseId = courseId
                    });
                }

                return;
            }

            if (previousStatus != Status.Completed)
            {
                return;
            }

            var hasOtherCompletedEnrollment = await _context.TraineeSessions.AnyAsync(e =>
                e.TraineeId == enrollment.TraineeId &&
                e.SessionId != enrollment.SessionId &&
                e.CourseSession.CourseId == courseId &&
                e.Status == Status.Completed);

            if (hasOtherCompletedEnrollment)
            {
                return;
            }

            var qualification = await _context.TraineeQualifications.FirstOrDefaultAsync(q =>
                q.TraineeId == enrollment.TraineeId &&
                q.CourseId == courseId);

            if (qualification is not null)
            {
                _context.TraineeQualifications.Remove(qualification);
            }
        }
    }
}

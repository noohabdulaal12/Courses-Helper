using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Hubs;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeSessionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<EnrollmentHub> _hubContext;

        public TraineeSessionsController(
            AppDbContext context,
            IHubContext<EnrollmentHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET all enrollments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TraineeSession>>> GetEnrollments()
        {
            return await _context.TraineeSessions.ToListAsync();
        }

        // ENROLL trainee into session
        [HttpPost]
        public async Task<ActionResult<TraineeSession>> Enroll(TraineeSession enrollment)
        {
            // check if already enrolled
            var existing = await _context.TraineeSessions
                .FirstOrDefaultAsync(x =>
                    x.TraineeId == enrollment.TraineeId &&
                    x.SessionId == enrollment.SessionId);

            if (existing != null)
            {
                return BadRequest("Trainee already enrolled.");
            }

            // find course session
            var session = await _context.CourseSessions
                .Include(s => s.TraineeSessions)
                .FirstOrDefaultAsync(s => s.Id == enrollment.SessionId);

            if (session == null)
            {
                return BadRequest("Session not found.");
            }

            // check seat availability
            if (session.TraineeSessions.Count >= session.MaxSeats)
            {
                return BadRequest("Session is full.");
            }

            // add enrollment
            _context.TraineeSessions.Add(enrollment);

            await _context.SaveChangesAsync();

            // calculate updated remaining seats
            var updatedSession = await _context.CourseSessions
                .Include(s => s.TraineeSessions)
                .FirstOrDefaultAsync(s => s.Id == enrollment.SessionId);

            int remainingSeats =
                updatedSession!.MaxSeats -
                updatedSession.TraineeSessions.Count;

            // send real-time update
            await _hubContext.Clients.All.SendAsync(
                "ReceiveSeatUpdate",
                enrollment.SessionId,
                remainingSeats
            );

            return Ok(enrollment);
        }

        // UPDATE enrollment status
        [HttpPut("{traineeId}/{sessionId}")]
        public async Task<IActionResult> UpdateStatus(
            string traineeId,
            int sessionId,
            TraineeSession updated)
        {
            var enrollment = await _context.TraineeSessions
                .FirstOrDefaultAsync(x =>
                    x.TraineeId == traineeId &&
                    x.SessionId == sessionId);

            if (enrollment == null)
            {
                return NotFound();
            }

            enrollment.Status = updated.Status;
            enrollment.AmountPaid = updated.AmountPaid;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE enrollment
        [HttpDelete("{traineeId}/{sessionId}")]
        public async Task<IActionResult> DeleteEnrollment(
            string traineeId,
            int sessionId)
        {
            var enrollment = await _context.TraineeSessions
                .FirstOrDefaultAsync(x =>
                    x.TraineeId == traineeId &&
                    x.SessionId == sessionId);

            if (enrollment == null)
            {
                return NotFound();
            }

            _context.TraineeSessions.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
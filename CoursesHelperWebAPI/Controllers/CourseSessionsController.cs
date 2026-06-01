using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseSessionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseSessionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseSession>>> GetSessions()
        {
            return await _context.CourseSessions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseSession>> GetSession(int id)
        {
            var session = await _context.CourseSessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            return session;
        }

        [HttpPost]
        public async Task<ActionResult<CourseSession>> CreateSession(CourseSession session)
        {
            // check instructor schedule conflict
            var instructorConflict = await _context.CourseSessions
                .AnyAsync(s =>
                    s.InstructorId == session.InstructorId &&
                    s.DayOfWeek == session.DayOfWeek &&
                    (
                        session.StartingTime < s.EndingTime &&
                        session.EndingTime > s.StartingTime
                    ));

            if (instructorConflict)
            {
                return BadRequest("Instructor is already assigned to another session during this time.");
            }

            // check classroom conflict
            var classroomConflict = await _context.CourseSessions
                .AnyAsync(s =>
                    s.ClassroomId == session.ClassroomId &&
                    s.DayOfWeek == session.DayOfWeek &&
                    (
                        session.StartingTime < s.EndingTime &&
                        session.EndingTime > s.StartingTime
                    ));

            if (classroomConflict)
            {
                return BadRequest("Classroom is already booked during this time.");
            }

            _context.CourseSessions.Add(session);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSession),
                new { id = session.Id }, session);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(int id, CourseSession session)
        {
            if (id != session.Id)
            {
                return BadRequest();
            }

            _context.Entry(session).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var session = await _context.CourseSessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            _context.CourseSessions.Remove(session);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET remaining seats
        [HttpGet("{id}/remaining-seats")]
        public async Task<IActionResult> GetRemainingSeats(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.TraineeSessions)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            int enrolled = session.TraineeSessions.Count;
            int remainingSeats = session.MaxSeats - enrolled;

            return Ok(new
            {
                maxSeats = session.MaxSeats,
                enrolled,
                remainingSeats
            });
        }
    }
}
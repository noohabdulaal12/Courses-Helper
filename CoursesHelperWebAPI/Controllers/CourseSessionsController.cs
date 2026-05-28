using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Authorization;

namespace CoursesHelperWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseSessionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseSessionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CourseSessions
        [HttpGet]
        public async Task<ActionResult> GetSessions()
        {
            var sessions = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                    .ThenInclude(c => c.ClassroomType)
                .Select(s => new
                {
                    s.Id,
                    s.CourseId,
                    CourseName = s.Course.Name,
                    s.InstructorId,
                    InstructorName = s.Instructor.UserName,
                    s.ClassroomId,
                    ClassroomDescription = s.Classroom.Description,
                    ClassroomType = s.Classroom.ClassroomType.Name,
                    s.MaxSeats,
                    s.StartingDate,
                    s.StartingTime,
                    s.EndingTime,
                    s.DayOfWeek
                })
                .ToListAsync();

            return Ok(sessions);
        }

        // GET: api/CourseSessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSession(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                    .ThenInclude(c => c.ClassroomType)
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.CourseId,
                    CourseName = s.Course.Name,
                    s.InstructorId,
                    InstructorName = s.Instructor.UserName,
                    s.ClassroomId,
                    ClassroomDescription = s.Classroom.Description,
                    ClassroomType = s.Classroom.ClassroomType.Name,
                    s.MaxSeats,
                    s.StartingDate,
                    s.StartingTime,
                    s.EndingTime,
                    s.DayOfWeek
                })
                .FirstOrDefaultAsync();

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        // POST: api/CourseSessions
        [HttpPost]
        public async Task<ActionResult> CreateSession(CourseSession session)
        {
            var course = await _context.Courses.FindAsync(session.CourseId);
            if (course == null)
                return BadRequest("Course does not exist.");

            var instructor = await _context.Users.FindAsync(session.InstructorId);
            if (instructor == null)
                return BadRequest("Instructor does not exist.");

            var classroom = await _context.Classrooms.FindAsync(session.ClassroomId);
            if (classroom == null)
                return BadRequest("Classroom does not exist.");

            bool classroomConflict = await _context.CourseSessions.AnyAsync(s =>
                s.ClassroomId == session.ClassroomId &&
                s.StartingDate == session.StartingDate &&
                session.StartingTime < s.EndingTime &&
                session.EndingTime > s.StartingTime);

            if (classroomConflict)
                return BadRequest("Classroom is already booked at that time.");

            bool instructorConflict = await _context.CourseSessions.AnyAsync(s =>
                s.InstructorId == session.InstructorId &&
                s.StartingDate == session.StartingDate &&
                session.StartingTime < s.EndingTime &&
                session.EndingTime > s.StartingTime);

            if (instructorConflict)
                return BadRequest("Instructor is already booked at that time.");

            _context.CourseSessions.Add(session);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
        }

        // PUT: api/CourseSessions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(int id, CourseSession session)
        {
            if (id != session.Id)
                return BadRequest("ID mismatch.");

            var existing = await _context.CourseSessions.FindAsync(id);
            if (existing == null)
                return NotFound();

            var course = await _context.Courses.FindAsync(session.CourseId);
            if (course == null)
                return BadRequest("Course does not exist.");

            var instructor = await _context.Users.FindAsync(session.InstructorId);
            if (instructor == null)
                return BadRequest("Instructor does not exist.");

            var classroom = await _context.Classrooms.FindAsync(session.ClassroomId);
            if (classroom == null)
                return BadRequest("Classroom does not exist.");

            bool classroomConflict = await _context.CourseSessions.AnyAsync(s =>
                s.Id != id &&
                s.ClassroomId == session.ClassroomId &&
                s.StartingDate == session.StartingDate &&
                session.StartingTime < s.EndingTime &&
                session.EndingTime > s.StartingTime);

            if (classroomConflict)
                return BadRequest("Classroom conflict.");

            bool instructorConflict = await _context.CourseSessions.AnyAsync(s =>
                s.Id != id &&
                s.InstructorId == session.InstructorId &&
                s.StartingDate == session.StartingDate &&
                session.StartingTime < s.EndingTime &&
                session.EndingTime > s.StartingTime);

            if (instructorConflict)
                return BadRequest("Instructor conflict.");

            existing.CourseId = session.CourseId;
            existing.InstructorId = session.InstructorId;
            existing.ClassroomId = session.ClassroomId;
            existing.MaxSeats = session.MaxSeats;
            existing.StartingDate = session.StartingDate;
            existing.StartingTime = session.StartingTime;
            existing.EndingTime = session.EndingTime;
            existing.DayOfWeek = session.DayOfWeek;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CourseSessions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var session = await _context.CourseSessions.FindAsync(id);

            if (session == null)
                return NotFound();

            _context.CourseSessions.Remove(session);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

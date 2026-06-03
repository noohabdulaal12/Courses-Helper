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
        public async Task<ActionResult> CreateSession(CreateCourseSessionDto dto)
        {
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
                return BadRequest("Course does not exist.");

            var instructor = await _context.Users.FindAsync(dto.InstructorId);
            if (instructor == null || instructor.UserType != UserType.Instructor)
                return BadRequest("Instructor does not exist.");

            var classroom = await _context.Classrooms.FindAsync(dto.ClassroomId);
            if (classroom == null)
                return BadRequest("Classroom does not exist.");

            bool classroomConflict = await _context.CourseSessions.AnyAsync(s =>
                s.ClassroomId == dto.ClassroomId &&
                s.StartingDate == dto.StartingDate &&
                dto.StartingTime < s.EndingTime &&
                dto.EndingTime > s.StartingTime);

            if (classroomConflict)
                return BadRequest("Classroom is already booked at that time.");

            bool instructorConflict = await _context.CourseSessions.AnyAsync(s =>
                s.InstructorId == dto.InstructorId &&
                s.StartingDate == dto.StartingDate &&
                dto.StartingTime < s.EndingTime &&
                dto.EndingTime > s.StartingTime);

            if (instructorConflict)
                return BadRequest("Instructor is already booked at that time.");

            if (!await InstructorCanTeachAsync(dto.InstructorId, dto.CourseId))
                return BadRequest("Instructor is not qualified to teach this course.");

            if (!await InstructorIsAvailableAsync(dto.InstructorId, dto.StartingDate, dto.StartingTime, dto.EndingTime))
                return BadRequest("Instructor is not available during the selected time.");

            var session = new CourseSession
            {
                CourseId = dto.CourseId,
                InstructorId = dto.InstructorId,
                ClassroomId = dto.ClassroomId,
                MaxSeats = dto.MaxSeats,
                StartingDate = dto.StartingDate,
                StartingTime = dto.StartingTime,
                EndingTime = dto.EndingTime,
                DayOfWeek = dto.StartingDate.DayOfWeek
            };

            _context.CourseSessions.Add(session);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
        }

        // PUT: api/CourseSessions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(int id, CreateCourseSessionDto dto)
        {
            var existing = await _context.CourseSessions.FindAsync(id);
            if (existing == null)
                return NotFound();

            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
                return BadRequest("Course does not exist.");

            var instructor = await _context.Users.FindAsync(dto.InstructorId);
            if (instructor == null || instructor.UserType != UserType.Instructor)
                return BadRequest("Instructor does not exist.");

            var classroom = await _context.Classrooms.FindAsync(dto.ClassroomId);
            if (classroom == null)
                return BadRequest("Classroom does not exist.");

            bool classroomConflict = await _context.CourseSessions.AnyAsync(s =>
                s.Id != id &&
                s.ClassroomId == dto.ClassroomId &&
                s.StartingDate == dto.StartingDate &&
                dto.StartingTime < s.EndingTime &&
                dto.EndingTime > s.StartingTime);

            if (classroomConflict)
                return BadRequest("Classroom conflict.");

            bool instructorConflict = await _context.CourseSessions.AnyAsync(s =>
                s.Id != id &&
                s.InstructorId == dto.InstructorId &&
                s.StartingDate == dto.StartingDate &&
                dto.StartingTime < s.EndingTime &&
                dto.EndingTime > s.StartingTime);

            if (instructorConflict)
                return BadRequest("Instructor conflict.");

            if (!await InstructorCanTeachAsync(dto.InstructorId, dto.CourseId))
                return BadRequest("Instructor is not qualified to teach this course.");

            if (!await InstructorIsAvailableAsync(dto.InstructorId, dto.StartingDate, dto.StartingTime, dto.EndingTime))
                return BadRequest("Instructor is not available during the selected time.");

            existing.CourseId = dto.CourseId;
            existing.InstructorId = dto.InstructorId;
            existing.ClassroomId = dto.ClassroomId;
            existing.MaxSeats = dto.MaxSeats;
            existing.StartingDate = dto.StartingDate;
            existing.StartingTime = dto.StartingTime;
            existing.EndingTime = dto.EndingTime;
            existing.DayOfWeek = dto.StartingDate.DayOfWeek;

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

        // GET: api/CourseSessions/5/remaining-seats
        [HttpGet("{id}/remaining-seats")]
        public async Task<IActionResult> GetRemainingSeats(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.TraineeSessions)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
                return NotFound();

            var enrolled = session.TraineeSessions.Count;
            var remainingSeats = session.MaxSeats - enrolled;

            return Ok(new
            {
                maxSeats = session.MaxSeats,
                enrolled,
                remainingSeats
            });
        }

        private async Task<bool> InstructorCanTeachAsync(string instructorId, int courseId)
        {
            return await _context.instructorQualifications.AnyAsync(q =>
                q.InstructorId == instructorId &&
                q.CourseId == courseId);
        }

        private async Task<bool> InstructorIsAvailableAsync(
            string instructorId,
            DateOnly startingDate,
            TimeSpan startingTime,
            TimeSpan endingTime)
        {
            return await _context.InstructorAvailabilities.AnyAsync(a =>
                a.InstructorId == instructorId &&
                a.DayOfWeek == startingDate.DayOfWeek &&
                a.StartingTime <= startingTime &&
                a.EndingTime >= endingTime);
        }
    }
}

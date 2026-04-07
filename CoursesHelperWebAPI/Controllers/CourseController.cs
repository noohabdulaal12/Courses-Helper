using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.DTOs;

namespace CoursesHelperWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Subject)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.SubjectId,
                    SubjectName = c.Subject.Name,
                    c.NumberOfSessions,
                    c.Price
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Subject)
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.SubjectId,
                    SubjectName = c.Subject.Name,
                    c.NumberOfSessions,
                    c.Price
                })
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound();

            return Ok(course);
        }

        // POST: api/Courses
        [HttpPost]
        public async Task<ActionResult> CreateCourse(CreateCourseDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            if (dto.Price < 0)
                return BadRequest("Price cannot be negative.");

            if (dto.NumberOfSessions <= 0)
                return BadRequest("Number of sessions must be greater than zero.");

            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == dto.SubjectId);
            if (!subjectExists)
                return BadRequest("Selected subject does not exist.");

            var course = new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                SubjectId = dto.SubjectId,
                NumberOfSessions = dto.NumberOfSessions,
                Price = dto.Price
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, new
            {
                course.Id,
                course.Name,
                course.Description,
                course.SubjectId,
                course.NumberOfSessions,
                course.Price
            });
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CreateCourseDto dto)
        {
            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            if (dto.Price < 0)
                return BadRequest("Price cannot be negative.");

            if (dto.NumberOfSessions <= 0)
                return BadRequest("Number of sessions must be greater than zero.");

            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == dto.SubjectId);
            if (!subjectExists)
                return BadRequest("Selected subject does not exist.");

            existingCourse.Name = dto.Name;
            existingCourse.Description = dto.Description;
            existingCourse.SubjectId = dto.SubjectId;
            existingCourse.NumberOfSessions = dto.NumberOfSessions;
            existingCourse.Price = dto.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
                return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
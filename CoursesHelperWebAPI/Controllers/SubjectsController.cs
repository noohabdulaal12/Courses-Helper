using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.DTOs;
using CoursesHelperWebAPI.Models.App;

namespace CoursesHelperWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult> GetSubjects()
        {
            var subjects = await _context.Subjects
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description
                })
                .ToListAsync();

            return Ok(subjects);
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSubject(int id)
        {
            var subject = await _context.Subjects
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description
                })
                .FirstOrDefaultAsync();

            if (subject == null)
                return NotFound();

            return Ok(subject);
        }

        // POST: api/Subjects
        [HttpPost]
        public async Task<ActionResult> CreateSubject(CreateSubjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            bool exists = await _context.Subjects.AnyAsync(s => s.Name == dto.Name);
            if (exists)
                return BadRequest("A subject with this name already exists.");

            var subject = new Subject
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubject), new { id = subject.Id }, new
            {
                subject.Id,
                subject.Name,
                subject.Description
            });
        }

        // PUT: api/Subjects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, CreateSubjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            var existing = await _context.Subjects.FindAsync(id);
            if (existing == null)
                return NotFound();

            bool duplicate = await _context.Subjects.AnyAsync(s => s.Id != id && s.Name == dto.Name);
            if (duplicate)
                return BadRequest("A subject with this name already exists.");

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
                return NotFound();

            if (subject.Courses.Any())
                return BadRequest("Cannot delete a subject that has courses assigned to it.");

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
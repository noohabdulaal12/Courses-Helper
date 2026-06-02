using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassroomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClassroomsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetClassrooms()
        {
            var classrooms = await _context.Classrooms
                .Include(c => c.ClassroomType)
                .Select(c => new
                {
                    c.Id,
                    c.TypeId,
                    ClassroomType = c.ClassroomType.Name,
                    Capacity = c.ClassroomType.Capacity,
                    c.Description
                })
                .ToListAsync();

            return Ok(classrooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetClassroom(int id)
        {
            var classroom = await _context.Classrooms
                .Include(c => c.ClassroomType)
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.TypeId,
                    ClassroomType = c.ClassroomType.Name,
                    Capacity = c.ClassroomType.Capacity,
                    c.Description
                })
                .FirstOrDefaultAsync();

            if (classroom == null)
            {
                return NotFound();
            }

            return Ok(classroom);
        }

        [HttpPost]
        public async Task<ActionResult<Classroom>> CreateClassroom(Classroom classroom)
        {
            if (!await _context.ClassroomsTypes.AnyAsync(t => t.Id == classroom.TypeId))
            {
                return BadRequest("Classroom type does not exist.");
            }

            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClassroom),
                new { id = classroom.Id }, classroom);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClassroom(int id, Classroom classroom)
        {
            if (id != classroom.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var existing = await _context.Classrooms.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (!await _context.ClassroomsTypes.AnyAsync(t => t.Id == classroom.TypeId))
            {
                return BadRequest("Classroom type does not exist.");
            }

            existing.TypeId = classroom.TypeId;
            existing.Description = classroom.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassroom(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);

            if (classroom == null)
            {
                return NotFound();
            }

            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

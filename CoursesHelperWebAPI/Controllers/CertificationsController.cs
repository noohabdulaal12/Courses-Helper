using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certification>>> GetCertifications()
        {
            return await _context.Certifications.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Certification>> GetCertification(int id)
        {
            var certification = await _context.Certifications.FindAsync(id);

            if (certification == null)
            {
                return NotFound();
            }

            return certification;
        }

        [HttpPost]
        public async Task<ActionResult<Certification>> CreateCertification(Certification certification)
        {
            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCertification),
                new { id = certification.Id }, certification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCertification(int id, Certification certification)
        {
            if (id != certification.Id)
            {
                return BadRequest();
            }

            _context.Entry(certification).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertification(int id)
        {
            var certification = await _context.Certifications.FindAsync(id);

            if (certification == null)
            {
                return NotFound();
            }

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
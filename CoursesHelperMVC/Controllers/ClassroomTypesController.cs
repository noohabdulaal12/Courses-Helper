using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoursesHelperMVC.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class ClassroomTypesController : Controller
    {
        private readonly AppDbContext _context;

        public ClassroomTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ClassroomTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClassroomsTypes.ToListAsync());
        }

        // GET: ClassroomTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classroomType = await _context.ClassroomsTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classroomType == null)
            {
                return NotFound();
            }

            return View(classroomType);
        }

        // GET: ClassroomTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClassroomTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Capacity,Description")] ClassroomType classroomType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classroomType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classroomType);
        }

        // GET: ClassroomTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classroomType = await _context.ClassroomsTypes.FindAsync(id);
            if (classroomType == null)
            {
                return NotFound();
            }
            return View(classroomType);
        }

        // POST: ClassroomTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Capacity,Description")] ClassroomType classroomType)
        {
            if (id != classroomType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classroomType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassroomTypeExists(classroomType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(classroomType);
        }

        // GET: ClassroomTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classroomType = await _context.ClassroomsTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classroomType == null)
            {
                return NotFound();
            }

            return View(classroomType);
        }

        // POST: ClassroomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classroomType = await _context.ClassroomsTypes.FindAsync(id);
            if (classroomType != null)
            {
                var isUsedByClassroom = await _context.Classrooms.AnyAsync(c => c.TypeId == id);
                if (isUsedByClassroom)
                {
                    ModelState.AddModelError(string.Empty, "This classroom type cannot be deleted because one or more classrooms use it.");
                    return View("Delete", classroomType);
                }

                _context.ClassroomsTypes.Remove(classroomType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassroomTypeExists(int id)
        {
            return _context.ClassroomsTypes.Any(e => e.Id == id);
        }
    }
}

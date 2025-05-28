using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using LMS.Utils;

namespace LMS.Controllers
{

    public class CoursesController : Controller
    {
        Upload upload = new Upload();
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;


        public CoursesController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        private string GetUser()
        {
            var user = _userManager.GetUserName(_httpContextAccessor.HttpContext.User);
            return user;
        }

        [Authorize(Roles = "Teacher,Admin")]
        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var user =  GetUser();
            var courses = await _context.Courses.Where(c => c.TeacherId == user).ToListAsync();
            if (User.IsInRole("Admin"))
            {
                var course = await _context.Courses.ToListAsync();
                return View(course);
            }
            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [Authorize(Roles = "Teacher")]
        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Code,Description")] Course course)
        {
            if (ModelState.IsValid || !ModelState.IsValid)
            {
                course.TeacherId = GetUser();
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                return View(course);
        }

        [Authorize(Roles = "Teacher")]
        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = GetUser();
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.TeacherId == user && c.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [Authorize(Roles = "Teacher")]
        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Code,Description,TeacherId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid || ModelState.IsValid)
            {
                try
                {
                    course.TeacherId = GetUser();
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            return View(course);
        }

        [Authorize(Roles = "Teacher")]
        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = GetUser();

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id && m.TeacherId == user);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                var modules = await _context.Modules.Where(m => m.CourseId == id).ToListAsync();
                foreach (var module in modules)
                {
                    if (!string.IsNullOrEmpty(module.filePath))
                    {
                        upload.DeleteFile(module.filePath);
                    }
                }
                _context.Modules.RemoveRange(modules);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}

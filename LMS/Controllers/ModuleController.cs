using LMS.Data;
using LMS.Models;
using LMS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class ModuleController : Controller
    {
        private readonly IModuleRepository _moduleRepo;
        private readonly ApplicationDbContext _context;
        public ModuleController(IModuleRepository moduleRepo, ApplicationDbContext context)
        {
            _moduleRepo = moduleRepo;
            _context = context;
        }
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> Index(int courseId)
        {
            ViewData["courseId"] = courseId;
            IEnumerable<Module> modules = await _moduleRepo.GetModulesByCourseId(courseId);
            return View(modules);
        }
        [Authorize(Roles = "Teacher")]
        public IActionResult AddModule(int courseId)
        {
            var model = new ModuleDTO
            {
                CourseId = courseId
            };
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName");
            return View(model);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddModule( [FromForm] ModuleDTO moduleDTO)
        {
            string? savedFilePath = null;

            if (moduleDTO.file != null)
            {
                var extension = Path.GetExtension(moduleDTO.file.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".pptx" };
                if (!allowedExtensions.Contains(extension.ToLower()))
                    return BadRequest("File type not allowed.");

                if (moduleDTO.file.Length > 5 * 1024 * 1024)
                    return BadRequest("File size exceeds 5MB.");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await moduleDTO.file.CopyToAsync(stream);
                }

                savedFilePath = Path.Combine("uploads", fileName);
            }
            var module = new Module
            {
                CourseId = moduleDTO.CourseId,
                Title = moduleDTO.Title,
                Description = moduleDTO.Description,
                ResourceUrl = moduleDTO.ResourceUrl,
                TypeId = moduleDTO.TypeId,
                filePath = savedFilePath
            };
            if (ModelState.IsValid)
            {
                await _moduleRepo.AddModule(module);
                return RedirectToAction("Index", new { courseId = module.CourseId });
            }
            return View(module);
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EditModule(int id)
        {
            var module = await _moduleRepo.GetModuleById(id);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName");
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> EditModule(Module module)
        {
            if (ModelState.IsValid)
            {
                await _moduleRepo.UpdateModule(module);
                return RedirectToAction("Index", new { courseId = module.CourseId });
            }
            return View(module);
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _moduleRepo.GetModuleById(id);
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost, ActionName("DeleteModule")]
        public async Task<IActionResult> DeleteModuleConfirmed(int id)
        {
            var module = await _moduleRepo.GetModuleById(id);
            if (module == null)
            {
                return NotFound();
            }
            await _moduleRepo.DeleteModule(module);
            return RedirectToAction("Index", new { courseId = module.CourseId });
        }
    }
}

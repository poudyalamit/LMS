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
    [Authorize(Roles = "Teacher")]
    public class ModuleController : Controller
    {
        private readonly IModuleRepository _moduleRepo;
        private readonly ApplicationDbContext _context;
        public ModuleController(IModuleRepository moduleRepo, ApplicationDbContext context)
        {
            _moduleRepo = moduleRepo;
            _context = context;
        }
        public async Task<IActionResult> Index(int courseId)
        {
            ViewData["courseId"] = courseId;
            IEnumerable<Module> modules = await _moduleRepo.GetModulesByCourseId(courseId);
            return View(modules);
        }

        public IActionResult AddModule(int courseId)
        {
            var model = new Module { CourseId = courseId };
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddModule(Module module)
        {
            if (ModelState.IsValid)
            {
                await _moduleRepo.AddModule(module);
                return RedirectToAction("Index", new { courseId = module.CourseId });
            }
            return View(module);
        }

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

        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _moduleRepo.GetModuleById(id);
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }
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

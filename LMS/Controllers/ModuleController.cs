using LMS.Data;
using LMS.Data.Migrations;
using LMS.Models;
using LMS.Repositories;
using LMS.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class ModuleController : Controller
    {
        Upload upload = new Upload();
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
               savedFilePath = await upload.UploadFile(moduleDTO.file);
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
            var moduleDTO = new ModuleDTO
            {
                CourseId = module.CourseId,
                Title = module.Title,
                Description = module.Description,
                ResourceUrl = module.ResourceUrl,
                TypeId = module.TypeId,
                FilePath = module.filePath
            };
            return View(moduleDTO);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> EditModule(int id, [FromForm] ModuleDTO moduleDTO)
        {
            var module = await _moduleRepo.GetModuleById(id);
            if (module == null)
            {
                return NotFound();
            }

            Upload upload = new Upload();
            string? savedFilePath = null;

            if (moduleDTO.file != null)
            {
                savedFilePath = await upload.EditFile(moduleDTO.file, module.filePath);
            }
            else
            {
                savedFilePath = module.filePath; 
            }

            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName");

            module.Title = moduleDTO.Title;
            module.Description = moduleDTO.Description;
            module.ResourceUrl = moduleDTO.ResourceUrl;
            module.TypeId = moduleDTO.TypeId;
            module.filePath = savedFilePath;

            if (ModelState.IsValid)
            {
                await _moduleRepo.UpdateModule(module);
                return RedirectToAction("Index", new { courseId = module.CourseId });
            }
            return View(module);
        }
        public async Task<IActionResult> Detail(int Id)
        {
            var module = await _moduleRepo.GetModuleById(Id);
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }
        [Authorize(Roles = "Teacher,Admin")]
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
            if (!string.IsNullOrEmpty(module.filePath))
            {
                upload.DeleteFile(module.filePath);
            }

            await _moduleRepo.DeleteModule(module);
            return RedirectToAction("Index", new { courseId = module.CourseId });
        }
    }
}

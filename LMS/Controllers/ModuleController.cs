using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class ModuleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

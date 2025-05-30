using LMS.Models;
using LMS.Models.DTO;
using LMS.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.Collections;
using System.Diagnostics;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _homeRepository = homeRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async  Task<IActionResult> Enroll(string s= "")
        {
            IEnumerable<Course> courses = await _homeRepository.GetAllCourses(s);
            DisplayCourses displayCourse = new DisplayCourses
            {
                Courses = courses,
                s = s
            };
            return View(displayCourse);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StdInfo()
        {
            IEnumerable<IdentityUser> student = await _userManager.GetUsersInRoleAsync("Student");
            ViewData["Role"] = "Student";
            return View(student);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TchrInfo()
        {
            IEnumerable<IdentityUser> teacher = await _userManager.GetUsersInRoleAsync("Teacher");
            ViewData["Role"] = "Teacher";
            return View("StdInfo", teacher);
        }
        //[Authorize(Roles = "Admin")]
        //public async Task<> DeleteUser(string UserId)
        //{

        //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

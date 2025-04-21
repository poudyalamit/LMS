using LMS.Models;
using LMS.Models.DTO;
using LMS.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public IActionResult Index()
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
            DisplayCourse displayCourse = new DisplayCourse
            {
                Courses = courses,
                s = s
            };
            return View(displayCourse);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

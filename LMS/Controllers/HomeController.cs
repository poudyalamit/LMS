
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        private readonly IEnrollRepository _enrollRepo;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository, UserManager<IdentityUser> userManager, IEnrollRepository enrollRepo)
        {
            _logger = logger;
            _homeRepository = homeRepository;
            _userManager = userManager;
            _enrollRepo = enrollRepo;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string UserEmail)
        {
            if (string.IsNullOrEmpty(UserEmail))
            {
                return BadRequest("User email cannot be null or empty.");
            }

            var user = await _userManager.FindByEmailAsync(UserEmail);
            if (user == null)
            {
                return NotFound($"User with email {UserEmail} not found.");
            }

            var isStudent = await _userManager.IsInRoleAsync(user, "Student");
            if (isStudent)
            {
                IEnumerable<Enrollment> enrollments = await _enrollRepo.GetEnrollmentByStudentId(UserEmail);
                foreach(var enrollment in enrollments)
                {
                    await _enrollRepo.DeleteEnrollment(enrollment);
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to delete the user.");
            }

            return RedirectToAction("StdInfo");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace LMS.Controllers
{
    public class EnrollController : Controller
    {
        private readonly IEnrollRepository _enrollRepo;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;


        public EnrollController(IEnrollRepository enrollRepo, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _enrollRepo = enrollRepo;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        private string GetUser()
        {
            var user = _userManager.GetUserName(_httpContextAccessor.HttpContext.User);
            return user;
        }

        public async Task<IEnumerable<Enrollment>> Index()
        {
            IEnumerable<Enrollment> enrollments = await _enrollRepo.GetAllEnrollments();
            return enrollments;
        }

        public async Task<IActionResult> EnrollStudent(int courseId, string teacherId)
        {
            var studentId  =  GetUser();
            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = studentId,
                TeacherId = teacherId
            };
            await _enrollRepo.AddEnrollment(enrollment);
            return RedirectToAction("Index", "Module", new { CourseId = courseId});
        }

        public async Task<IActionResult> DerollStudent(int courseId)
        {
            var Id = await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.StudentId == GetUser())
                .Select(e => e.Id)
                .FirstOrDefaultAsync();
            var enrollment = await _enrollRepo.GetEnrollmentById(Id);
            if (enrollment != null)
            {
                await _enrollRepo.DeleteEnrollment(enrollment);
                return RedirectToAction("Enroll", "Home");
            }
            return NotFound();
        }

        public async Task<IActionResult> CourseEnrollments(int courseId)
        {
            IEnumerable<Enrollment> enrollments = await _enrollRepo.GetEnrollmentsByCourseId(courseId);
            return View(enrollments);
        }

        public async Task<IActionResult> TeacherCourseEnrollment (string teacherId)
        {
            IEnumerable<Enrollment> enrollments = await _enrollRepo.GetEnrollmentByTeacherId(teacherId);
            return View(enrollments);
        }
        public async Task<IActionResult> StudentDashboard(string studentId)
        {
            IEnumerable<Enrollment> enrollments = await _enrollRepo.GetEnrollmentByStudentId(studentId);
            return View(enrollments);
        }
    }
}

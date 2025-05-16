
using Microsoft.AspNetCore.Identity;

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

        public async Task<IActionResult> DerollStudent(int Id)
        {
            var enrollment = await _enrollRepo.GetEnrollmentById(Id);
            if (enrollment != null)
            {
                await _enrollRepo.DeleteEnrollment(enrollment);
                return RedirectToAction("Enroll", "Home");
            }
            return NotFound();
        }

        public async Task<IActionResult> GetEnrollmentsByCourseId(int courseId)
        {
            IEnumerable<Enrollment> enrollments = await _enrollRepo.GetEnrollmentsByCourseId(courseId);
            return Ok(enrollments);
        }

        public async Task<IActionResult> GetEnrollmentsByTeacherId(string teacherId)
        {
            IEnumerable<Enrollment> enrollments = await _enrollRepo.GetEnrollmentByTeacherId(teacherId);
            return Ok(enrollments);
        }

        public async Task<IActionResult> GetEnrollmentsByStudentId(string studentId)
        {
            IEnumerable<Enrollment> enrollments = await _enrollRepo.GetEnrollmentByStudentId(studentId);
            return Ok(enrollments);
        }
    }
}

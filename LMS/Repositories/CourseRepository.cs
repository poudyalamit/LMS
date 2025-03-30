using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS.Repositories
{
    public class CourseRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseRepository(ApplicationDbContext db, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetUserId()
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            return userId;
        }
        public IEnumerable<Course> GetAllCourses()
        {
            return _db.Courses.ToList();
        }
        public Course GetCourseByCode(string Code)
        {
            var courses = _db.Courses.FirstOrDefault(c => c.Code == Code);
            return courses;
        }
        public async Task AddCourse(Course course)
        {
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateCourse(Course course)
        {
            _db.Courses.Update(course);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteCourse(Course course)
        {
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
        }

        public async Task<Course> GetCourseById(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            return course;
        }



    }
}

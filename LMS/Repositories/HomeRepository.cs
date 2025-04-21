using LMS.Data;
using LMS.Data.Migrations;
using LMS.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;
        public HomeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllCourses(string s = "")
        {
            s = s.ToLower();
            IEnumerable<Course> courses = await _context.Courses.Where(c => c.Title.ToLower().Contains(s) || c.Code.ToLower().Contains(s) || c.TeacherId.ToLower().Contains(s)).ToListAsync();
            return courses;
        }
    }

    public interface IHomeRepository
    {
        Task<IEnumerable<Course>> GetAllCourses(string s = "");
    }
}

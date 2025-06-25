
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;

namespace LMS.Repositories
{
    [Authorize(Roles = "Teacher,Admin")]
    public class ModuleRepository : IModuleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ModuleRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private async Task<string?> GetUserIdByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return user.Id;
            }

            return null;
        }

        public async Task AddModule (Module module)
        {
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Module>> GetModulesByCourseId(int courseId)
        {
            IEnumerable<Module> modules = await _context.Modules.Where(m => m.CourseId == courseId).Include(m=>m.Type).OrderByDescending(m => m.Id).ToListAsync();
            return modules;
        }

        public async Task<Module?> GetModuleById(int id)
        {
            return await _context.Modules.FindAsync(id);
        }

        public async Task UpdateModule(Module module)
        {
            _context.Modules.Update(module);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteModule(Module module)
        {
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
        }

        public Task<string?> GetModulesByCourseId()
        {
            throw new NotImplementedException();
        }

        public async Task StoreNotificationByCourse(string groupName, string message)
        {
            if (!groupName.StartsWith("course-")) return;

            var courseIdStr = groupName.Replace("course-", "");
            if (!int.TryParse(courseIdStr, out int courseId)) return;

            var userIds = await _context.Enrollments
                              .Where(e => e.CourseId == courseId)
                              .Select(e => e.StudentId)
                              .Distinct()
                              .ToListAsync();

            foreach (var userId in userIds)
            {
                var userIdByEmail = await GetUserIdByEmail(userId); 
                if (userIdByEmail == null) continue;
                var notification = new Notification
                {
                    UserId = userIdByEmail,
                    Message = message,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notification.Add(notification);
            }
            await _context.SaveChangesAsync();
        }
    }

    public interface IModuleRepository
    {
        Task AddModule(Module module);
        Task<IEnumerable<Module>> GetModulesByCourseId(int courseId);
        Task<Module?> GetModuleById(int id);
        Task UpdateModule(Module module);
        Task DeleteModule(Module module);
        Task StoreNotificationByCourse(string groupName, string message);
    }
}

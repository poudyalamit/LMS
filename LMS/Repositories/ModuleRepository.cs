
namespace LMS.Repositories
{
    [Authorize(Roles = "Teacher,Admin")]
    public class ModuleRepository : IModuleRepository
    {
        private readonly ApplicationDbContext _context;
        public ModuleRepository(ApplicationDbContext context)
        {
            _context = context;
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
    }

    public interface IModuleRepository
    {
        Task AddModule(Module module);
        Task<IEnumerable<Module>> GetModulesByCourseId(int courseId);
        Task<Module?> GetModuleById(int id);
        Task UpdateModule(Module module);
        Task DeleteModule(Module module);
    }
}

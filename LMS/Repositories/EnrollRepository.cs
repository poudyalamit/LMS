namespace LMS.Repositories
{
    public class EnrollRepository: IEnrollRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GetEnrollment()
        {
            //var enrollments = await _context.e
        }
    }

    public interface IEnrollRepository
    {

    }
}

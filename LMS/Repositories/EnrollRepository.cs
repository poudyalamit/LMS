namespace LMS.Repositories
{
    public class EnrollRepository : IEnrollRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<Enrollment>> GetAllEnrollments()
        {
            IEnumerable<Enrollment> enrollments = await _context.Enrollments.ToListAsync();
            return enrollments;
        }
        [Authorize(Roles = "Teacher")]
        public async Task<Enrollment?> GetEnrollmentById(int id)
        {
            return await _context.Enrollments.FindAsync(id);
        }
        [Authorize(Roles = "Teacher")]
        public async Task AddEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        // This method is not used in the current implementation and this is not need to be used but maybe used in the future
        public async Task UpdateEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }
        [Authorize(Roles = "Student")]
        public async Task DeleteEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseId(int courseId)
        {
            IEnumerable<Enrollment> enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
            return enrollments;
        }
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IEnumerable<Enrollment>> GetEnrollmentByTeacherId(string TeacherId)
        {
            IEnumerable<Enrollment> enrollments = await _context.Enrollments
                .Where(e => e.TeacherId == TeacherId)
                .ToListAsync();
            return enrollments;
        }
    }

    public interface IEnrollRepository
    {
        Task<IEnumerable<Enrollment>> GetAllEnrollments();
        Task<Enrollment?> GetEnrollmentById(int id);
        Task AddEnrollment(Enrollment enrollment);
        Task UpdateEnrollment(Enrollment enrollment);
        Task DeleteEnrollment(Enrollment enrollment);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseId(int courseId);
    }
}

using Microsoft.EntityFrameworkCore;

namespace LMS.Repositories
{
    public class EnrollRepository : IEnrollRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetAllEnrollments()
        {
            IEnumerable<Enrollment> enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .ToListAsync();
            return enrollments;
        }

        public async Task<Enrollment?> GetEnrollmentById(int id)
        {
            return await _context.Enrollments.FindAsync(id);
        }

        public async Task AddEnrollment(Enrollment enrollment)
        {
            var exists = await _context.Enrollments
            .AnyAsync(e => e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId);

            if (!exists)
            {
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task GetStudentCourse(string StdId)
        {
            await _context.Enrollments
                .Where(e => e.StudentId == StdId)
                .Select(e => e.CourseId)
                .ToListAsync();
        }

        // This method is not used in the current implementation and this is not need to be used but maybe used in the future
        public async Task UpdateEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseId(int courseId)
        {
            IEnumerable<Enrollment> enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
            return enrollments;
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentByTeacherId(string TeacherId)
        {
            IEnumerable<Enrollment> enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.TeacherId == TeacherId)
                .ToListAsync();
            return enrollments;
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentByStudentId(string StudentId)
        {
            IEnumerable<Enrollment> enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == StudentId)
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
        Task<IEnumerable<Enrollment>> GetEnrollmentByTeacherId(string TeacherId);
        Task<IEnumerable<Enrollment>> GetEnrollmentByStudentId(string StudentId);
    }
}

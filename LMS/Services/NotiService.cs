using System.Collections;

public class NotiService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotiService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable> GetStudentCourseIdsAsync(string studentId)
    {
        return (IQueryable)await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .ToListAsync();
    }
}

public interface INotificationService
{
    Task<IQueryable> GetStudentCourseIdsAsync(string studentId);
}
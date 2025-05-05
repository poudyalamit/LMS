namespace LMS.Models.DTO
{
    public class DisplayCourses
    {
     public IEnumerable<Course> Courses { get; set; }

     public int CourseId { get; set; }

     public string s { get; set; } = " ";
    }
}

namespace LMS.Models.DTO
{
    public class DisplayModules
    {
        public IEnumerable<Module> Modules { get; set; }

        public int CourseId { get; set; }
    }
}

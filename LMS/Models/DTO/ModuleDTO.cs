using System.ComponentModel.DataAnnotations;

namespace LMS.Models.DTO
{
    public class ModuleDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ResourceUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public IFormFile? file { get; set; }
        public int TypeId { get; set; }
    }
}

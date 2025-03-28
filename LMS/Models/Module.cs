using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("Modules")]
    public class Module
    {
        public int Id { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Title { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
        [MaxLength(100)]
        public string? ResourceUrl { get; set; }

        public Course? Course { get; set; }
    }
}

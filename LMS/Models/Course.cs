using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("Courses")]
    public class Course
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Title { get; set; }
        [Required]
        [MaxLength(20)]
        public string? Code { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
        [Required]
        public string? TeacherId { get; set; }
        public List<Module>? Modules { get; set; }
        public List<Enrollment>? Enrollments { get; set; }

    }
}

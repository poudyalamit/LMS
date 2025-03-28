using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("Enrollments")]
    public class Enrollment
    {
        public int Id { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string? StudentId { get; set; }
        public Course? Course { get; set; }
    }
}

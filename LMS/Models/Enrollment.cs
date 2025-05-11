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
        [Required]
        public string? TeacherId { get; set; }
        [Required]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public Course? Course { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Types
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string? TypeName { get; set; }
        public List<Module> ?Module { get; set; }
    }
}

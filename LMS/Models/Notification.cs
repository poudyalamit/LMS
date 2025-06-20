using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Notification
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(20)]
        public required string Title { get; set; }
        [MaxLength(50)]
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}

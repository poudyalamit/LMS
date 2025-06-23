using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Notification
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(200)]
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;
        public required string UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}

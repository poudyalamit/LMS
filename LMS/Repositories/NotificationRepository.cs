using LMS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace LMS.Repositories
{
    
    //These are not the perfect ways to implement Notification system. This is not optimal and can be improved further.
    
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<NotiHub> _hubContext;

        public NotificationRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<NotiHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public async Task SendNotification(string groupName, string message)
        {
            List<string> userIds = new List<string>();
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            var students = await _userManager.GetUsersInRoleAsync("Student");

            if (groupName == "Teachers")
            {
                userIds = teachers.Select(t => t.Id).ToList();
            }
            else if (groupName == "Students")
            {
                userIds = students.Select(s => s.Id).ToList();
            }
            else
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var adminIds = admins.Select(a => a.Id).ToHashSet();

                 userIds = _userManager.Users
                                       .Where(u => !adminIds.Contains(u.Id))
                                       .Select(u => u.Id)
                                       .ToList();
            }


                foreach (var userId in userIds)
                {
                    var notification = new Notification
                    {
                        UserId = userId,
                        Message = message,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };

                    _context.Notification.Add(notification);
                }

            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }

    }

    public interface INotificationRepository
    {
        Task SendNotification(string groupName, string message);
    }
}

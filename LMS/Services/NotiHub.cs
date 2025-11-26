using Microsoft.AspNetCore.SignalR;

namespace LMS.Services
{
    [Authorize]
    public class NotiHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notification;

        public NotiHub(INotificationService notification, ApplicationDbContext context)
        {
            _notification = notification;
            _context = context;
        }
        public async Task SendMessage(string user, string message)
        {
            var sender = Context?.User?.Identity?.Name;
            await Clients.Client(user).SendAsync("ReceiveNotification", sender, message);
        }

        public async Task SendNotiToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User != null && !Context.User.IsInRole("Admin"))
            {
                if (Context.User.IsInRole("Teacher"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Teachers");
                    await Groups.AddToGroupAsync(Context.ConnectionId, "All");
                }
                else if (Context.User.IsInRole("Student"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "All");
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Students");
                    var userId = Context?.User?.Identity?.Name;
                    var courseIds = await _notification.GetStudentCourseIdsAsync(userId);
                    if (courseIds == null)
                    {
                        return ;
                    }
                    var tasks = courseIds.ToString().Select(courseId =>
                        Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}")
                    );
                    await Task.WhenAll(tasks);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}

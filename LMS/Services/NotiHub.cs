using Microsoft.AspNetCore.SignalR;

namespace LMS.Services
{
    [Authorize]
    public class NotiHub : Hub
    {
        private readonly INotificationService _notification;

        public NotiHub(INotificationService notification)
        {
            _notification = notification;
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
            if (Context.User != null)
            {
                if (Context.User.IsInRole("Teacher"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Teachers");
                }
                else if (Context.User.IsInRole("Student"))
                {
                    var userId = Context?.User?.Identity?.Name;
                    var courseIds = await _notification.GetStudentCourseIdsAsync(userId);

                    foreach (var courseId in courseIds)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
                    }
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

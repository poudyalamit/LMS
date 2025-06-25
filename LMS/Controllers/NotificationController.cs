using LMS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<NotiHub> _hubContext;
        private readonly INotificationRepository _notiRepo;
        public NotificationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<NotiHub> hubContext, INotificationRepository notiRepo)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
            _notiRepo = notiRepo;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            if(notifications == null) 
            {
                ViewBag.NotificationCount = 0; 
            }
            else
            {
                ViewBag.NotificationCount = notifications?.Count(n => !n.IsRead);
            }
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification != null)
            {
                _context.Notification.Remove(notification);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAll()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var notifications = await _context.Notification.Where(n => n.UserId == userId).ToListAsync();
                _context.Notification.RemoveRange(notifications);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Create(string userId, string message)
        {
            if (userId != null && message != null)
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                _context.Notification.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Teacher,Student")]
        public async Task<int> GetUnreadCount()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            { 
                return 0;
            }
            int unreadCount = await _context.Notification
                .CountAsync(n => n.UserId == userId && !n.IsRead);
            return unreadCount;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Announce(string groupName, string message)
        {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(message))
            {
                return BadRequest("Group name and message cannot be empty.");
            }
            if( groupName == "Teachers" || groupName == "Students")
            {
                await _notiRepo.SendNotification(groupName,message);
            }
            else
            {
                await _notiRepo.SendNotificationAllUsers(message);
            }

            return View();
        }
    }
}

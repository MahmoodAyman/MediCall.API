using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var uid = User.FindFirst("sub")?.Value;
            if (uid == null)
            {
                return Unauthorized();
            }
            var notifications = await _notificationService.GetNotificationsAsync(uid);
            return Ok(notifications);
        }

        [HttpPost]
        [Route("mark-as-read")]
        public async Task<IActionResult> MarkNotificationAsRead([FromBody] int notificationId)
        {
            var uid = User.FindFirst("sub")?.Value;
            if (uid == null)
            {
                return Unauthorized();
            }
            var result = await _notificationService.MakeNotificationReadAsync(notificationId, uid);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}

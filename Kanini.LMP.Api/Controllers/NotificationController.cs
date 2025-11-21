using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IEnhancedNotificationService _enhancedNotificationService;

        public NotificationController(
            INotificationService notificationService,
            IEnhancedNotificationService enhancedNotificationService)
        {
            _notificationService = notificationService;
            _enhancedNotificationService = enhancedNotificationService;
        }

        [HttpGet("my-notifications")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetMyNotifications()
        {
            var userId = GetCurrentUserId();
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetUnreadNotifications()
        {
            var userId = GetCurrentUserId();
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }

        [HttpPut("mark-read/{notificationId}")]
        public async Task<ActionResult<NotificationDTO>> MarkAsRead(int notificationId)
        {
            var notification = await _notificationService.MarkAsReadAsync(notificationId);
            return Ok(notification);
        }

        [HttpPut("mark-all-read")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { success = result, message = "All notifications marked as read" });
        }

        // Notification Preferences
        [HttpGet("preferences")]
        public async Task<ActionResult> GetNotificationPreferences()
        {
            var userId = GetCurrentUserId();
            var preferences = await _enhancedNotificationService.GetUserNotificationPreferencesAsync(userId);
            return Ok(preferences);
        }

        [HttpPut("preferences/{notificationType}")]
        public async Task<ActionResult> UpdateNotificationPreferences(
            NotificationType notificationType,
            [FromBody] NotificationPreferenceUpdateDto request)
        {
            var userId = GetCurrentUserId();
            var result = await _enhancedNotificationService.UpdateNotificationPreferencesAsync(
                userId, notificationType, request.EmailEnabled, request.SMSEnabled, 
                request.PushEnabled, request.WhatsAppEnabled, request.InAppEnabled);

            return result ? Ok(new { Message = "Preferences updated successfully" }) 
                         : BadRequest("Failed to update preferences");
        }

        [HttpPost("test")]
        public async Task<ActionResult> SendTestNotification([FromBody] TestNotificationRequest request)
        {
            var userId = GetCurrentUserId();
            await _enhancedNotificationService.SendMultiChannelNotificationAsync(
                userId, request.Title ?? "Test Notification", 
                request.Message ?? "This is a test notification.",
                NotificationType.General, NotificationPriority.Low);

            return Ok(new { Message = "Test notification sent" });
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> SendBulkNotification([FromBody] BulkNotificationRequest request)
        {
            if (!request.UserIds.Any()) return BadRequest("User IDs required");

            await _enhancedNotificationService.SendBulkNotificationAsync(
                request.UserIds, request.Title, request.Message, request.Type, request.Priority);

            return Ok(new { Message = $"Sent to {request.UserIds.Count} users" });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }

    public class NotificationPreferenceUpdateDto
    {
        public bool EmailEnabled { get; set; } = true;
        public bool SMSEnabled { get; set; } = true;
        public bool PushEnabled { get; set; } = false;
        public bool WhatsAppEnabled { get; set; } = false;
        public bool InAppEnabled { get; set; } = true;
    }

    public class TestNotificationRequest
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
    }

    public class BulkNotificationRequest
    {
        public List<int> UserIds { get; set; } = new();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.General;
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;
    }
    }
}
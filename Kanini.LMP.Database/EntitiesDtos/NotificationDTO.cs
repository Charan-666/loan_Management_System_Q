using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto
{
    public class NotificationDTO
    {
        /// <summary>
        /// Unique identifier for the notification.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// ID of the user who receives this notification.
        /// </summary>
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        /// <summary>
        /// Short title of the notification.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Main message content of the notification.
        /// </summary>
        [Required(ErrorMessage = "Message is required")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; } = null!;

        /// <summary>
        /// Indicates whether the notification has been read by the user.
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Date and time the notification was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional: User's full name (for displaying in response).
        /// </summary>
        public string? UserName { get; set; }
    }
}

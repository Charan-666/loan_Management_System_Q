using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Database.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; } 
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = null!;
        [Required]
        public bool IsRead { get; set; } = false;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
        public NotificationType Type { get; set; } = NotificationType.General;
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;
        public bool IsSent { get; set; } = false;
        public DateTime? SentAt { get; set; }
        public string? ExternalId { get; set; } // For tracking SMS/WhatsApp message IDs
        public User? User { get; set; }
    }

    public class NotificationPreference
    {
        [Key]
        public int PreferenceId { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public bool EmailEnabled { get; set; } = true;
        public bool SMSEnabled { get; set; } = true;
        public bool PushEnabled { get; set; } = true;
        public bool WhatsAppEnabled { get; set; } = false;
        public bool InAppEnabled { get; set; } = true;
        public User? User { get; set; }
    }
}

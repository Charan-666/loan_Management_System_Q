using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public User? User { get; set; }

    }
}

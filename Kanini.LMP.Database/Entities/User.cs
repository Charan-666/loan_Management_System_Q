using Kanini.LMP.Database.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        [DisplayName("Full Name")]
        public string FullName { get; set; } = null!;

        [DisplayName("Email")]
        [Required(ErrorMessage = "The Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required]
        [DisplayName("Password")]
        public string PasswordHash { get; set; } = null!;

        // Role of the user (Admin, Manager, Customer)

        [Required]
        public UserEnums Roles { get; set; }

        // Account status (Active, Inactive, Locked)
        [Required]

        public UserStatus Status { get; set; }

        // Audit fields
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get;set; }
    }
}

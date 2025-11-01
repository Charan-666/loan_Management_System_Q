using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto
{
    public class UserDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        [DisplayName("Full Name")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        [DisplayName("Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [DisplayName("Password")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "User Role is required")]
        [DisplayName("Role")]
        public UserEnums Roles { get; set; }

        [Required(ErrorMessage = "User Status is required")]
        [DisplayName("Account Status")]
        public UserStatus Status { get; set; }

        [DisplayName("Created Date")]
        public DateTime? CreatedAt { get; set; }

        [DisplayName("Last Updated Date")]
        public DateTime? UpdatedAt { get; set; }
    }
}

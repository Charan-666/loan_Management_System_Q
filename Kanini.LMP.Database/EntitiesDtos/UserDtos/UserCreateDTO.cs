using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.User
{
    public class UserCreateDTO
    {
        [Required, MaxLength(100)]
        [DisplayName("Full Name")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "The Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [DisplayName("Email Address")]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DisplayName("Password")]
        public string Password { get; set; } = null!;

        [Required]
        [DisplayName("User Role")]
        public UserEnums Roles { get; set; }

        [Required]
        [DisplayName("Account Status")]
        public UserStatus Status { get; set; } = UserStatus.Active;
    }
}

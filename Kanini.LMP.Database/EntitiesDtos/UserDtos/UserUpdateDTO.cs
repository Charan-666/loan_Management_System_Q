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
    public class UserUpdateDTO
    {
        [Required]
        [DisplayName("User ID")]
        public int UserId { get; set; }

        [MaxLength(100)]
        [DisplayName("Full Name")]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [DisplayName("Email Address")]
        public string? Email { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DisplayName("Password")]
        public string? Password { get; set; }

        [DisplayName("User Role")]
        public UserEnums? Roles { get; set; }

        [DisplayName("Account Status")]
        public UserStatus? Status { get; set; }
    }
}

using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.User
{
    public class UserResponseDTO
    {

        [DisplayName("User ID")]
        public int UserId { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; } = null!;

        [DisplayName("Email Address")]
        public string Email { get; set; } = null!;

        [DisplayName("User Role")]
        public UserEnums Roles { get; set; }

        [DisplayName("Account Status")]
        public UserStatus Status { get; set; }

        [DisplayName("Created Date")]
        public DateTime? CreatedAt { get; set; }

        [DisplayName("Last Updated")]
        public DateTime? UpdatedAt { get; set; }
    }
}

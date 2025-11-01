using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer
{
    public class CustomerCreateDTO
    {
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian phone number.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Occupation is required.")]
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;

        [Required(ErrorMessage = "Annual Income is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Annual income must be positive.")]
        public decimal AnnualIncome { get; set; }

        [Required(ErrorMessage = "Credit Score is required.")]
        [Range(0, 900, ErrorMessage = "Credit score must be between 0 and 900.")]
        public decimal CreditScore { get; set; }

        [Required(ErrorMessage = "Profile image is required.")]
        public byte[] ProfileImage { get; set; } = null!;

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }
    }
}

using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [Required]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian phone number.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal AnnualIncome { get; set; }

        [Range(0, 900)]
        public decimal CreditScore { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int Age { get; set; }

        // Optional: include this if your API supports showing profile images (Base64 encoded)
        public string? ProfileImageBase64 { get; set; }

        // Optional: to include loan application IDs or summary
        public ICollection<int>? ApplicationIds { get; set; }
    }
}

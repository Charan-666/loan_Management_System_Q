using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer
{
    public class CustomerUpdateDTO
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian phone number.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Occupation is required.")]
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;

        [Required(ErrorMessage = "Annual Income is required.")]
        [Range(0, double.MaxValue)]
        public decimal AnnualIncome { get; set; }

        [Required(ErrorMessage = "Credit Score is required.")]
        [Range(0, 900)]
        public decimal CreditScore { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }

        public byte[]? ProfileImage { get; set; }
    }
}

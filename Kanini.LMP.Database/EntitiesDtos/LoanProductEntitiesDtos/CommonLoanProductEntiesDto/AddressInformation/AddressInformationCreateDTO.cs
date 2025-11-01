using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation
{
    public class AddressInformationCreateDTO
    {
        [Required(ErrorMessage = "Loan Application Base ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required, MaxLength(250)]
        public string PresentAddress { get; set; } = null!;

        [Required, MaxLength(250)]
        public string PermanentAddress { get; set; } = null!;

        [Required, MaxLength(100)]
        public string District { get; set; } = null!;

        [Required]
        public IndianStates State { get; set; }

        [Required, MaxLength(100)]
        public string Country { get; set; } = null!;

        [Required, MaxLength(10)]
        [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Enter a valid zip code.")]
        public string ZipCode { get; set; } = null!;

        [Required, EmailAddress, MaxLength(100)]
        public string EmailId { get; set; } = null!;

        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string MobileNumber1 { get; set; } = null!;

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string? MobileNumber2 { get; set; }

    }
}

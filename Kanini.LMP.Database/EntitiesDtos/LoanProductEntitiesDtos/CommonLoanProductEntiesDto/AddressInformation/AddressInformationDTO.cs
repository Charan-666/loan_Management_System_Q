using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation
{
    public class AddressInformationDTO
    {
        // Primary Key (optional for input DTOs, useful for updates)
        public int AddressInformationId { get; set; }

        // Foreign keys (usually handled internally, but may be needed for admin operations)
        public int LoanApplicationBaseId { get; set; }
        public int UserId { get; set; }

        // --- Address Details ---
        [Required(ErrorMessage = "Present address is required.")]
        [MaxLength(250, ErrorMessage = "Present address cannot exceed 250 characters.")]
        public string PresentAddress { get; set; } = null!;

        [Required(ErrorMessage = "Permanent address is required.")]
        [MaxLength(250, ErrorMessage = "Permanent address cannot exceed 250 characters.")]
        public string PermanentAddress { get; set; } = null!;

        // --- Location ---
        [Required(ErrorMessage = "District is required.")]
        [MaxLength(100, ErrorMessage = "District cannot exceed 100 characters.")]
        public string District { get; set; } = null!;

        [Required(ErrorMessage = "State is required.")]
        public IndianStates State { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string Country { get; set; } = null!;

        [Required(ErrorMessage = "Zip code is required.")]
        [MaxLength(10, ErrorMessage = "Zip code cannot exceed 10 characters.")]
        [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Enter a valid zip code.")]
        public string ZipCode { get; set; } = null!;

        // --- Contact Details ---
        [Required(ErrorMessage = "Email ID is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(100)]
        public string EmailId { get; set; } = null!;

        [Required(ErrorMessage = "Primary mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string MobileNumber1 { get; set; } = null!;  // changed to string for validation and flexibility

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string? MobileNumber2 { get; set; }  // optional secondary number
    }
}

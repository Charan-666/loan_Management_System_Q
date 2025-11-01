using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails
{
    public class PersonalDetailsCreateDTO
    {
        [Required(ErrorMessage = "Loan Application Base ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "District of birth is required.")]
        [MaxLength(100, ErrorMessage = "District name cannot exceed 100 characters.")]
        public string DistrictOfBirth { get; set; } = null!;

        [Required(ErrorMessage = "Country of birth is required.")]
        [MaxLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string CountryOfBirth { get; set; } = null!;

        [Required(ErrorMessage = "PAN number is required.")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN number format.")]
        [MaxLength(10)]
        public string PANNumber { get; set; } = null!;

        [Required(ErrorMessage = "Education qualification is required.")]
        [MaxLength(100)]
        public string EducationQualification { get; set; } = null!;

        [Required(ErrorMessage = "Residential status is required.")]
        [MaxLength(50)]
        public string ResidentialStatus { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Signature image is required.")]
        public byte[] SignatureImage { get; set; } = null!;

        [Required(ErrorMessage = "ID proof image is required.")]
        public byte[] IDProofImage { get; set; } = null!;
    }
}

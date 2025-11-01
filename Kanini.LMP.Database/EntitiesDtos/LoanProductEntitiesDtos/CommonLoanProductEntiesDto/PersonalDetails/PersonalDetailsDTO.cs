using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails
{
    public class PersonalDetailsDTO
    {
        /// <summary>
        /// Unique identifier for the personal details record.
        /// </summary>
        public int PersonalDetailsId { get; set; }

        /// <summary>
        /// Foreign key reference to the Loan Application Base entity.
        /// </summary>
        [Required(ErrorMessage = "Loan Application Base ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        /// <summary>
        /// Foreign key reference to the User entity.
        /// </summary>
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        /// <summary>
        /// Full name of the applicant.
        /// </summary>
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Date of birth of the applicant.
        /// </summary>
        [Required(ErrorMessage = "Date of birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// District where the applicant was born.
        /// </summary>
        [Required(ErrorMessage = "District of birth is required.")]
        [MaxLength(100, ErrorMessage = "District name cannot exceed 100 characters.")]
        public string DistrictOfBirth { get; set; } = null!;

        /// <summary>
        /// Country where the applicant was born.
        /// </summary>
        [Required(ErrorMessage = "Country of birth is required.")]
        [MaxLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string CountryOfBirth { get; set; } = null!;

        /// <summary>
        /// PAN number (Permanent Account Number) of the applicant.
        /// </summary>
        [Required(ErrorMessage = "PAN number is required.")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN number format.")]
        [MaxLength(10, ErrorMessage = "PAN number must be 10 characters.")]
        public string PANNumber { get; set; } = null!;

        /// <summary>
        /// Highest education qualification of the applicant.
        /// </summary>
        [Required(ErrorMessage = "Education qualification is required.")]
        [MaxLength(100, ErrorMessage = "Education qualification cannot exceed 100 characters.")]
        public string EducationQualification { get; set; } = null!;

        /// <summary>
        /// Current residential status of the applicant.
        /// </summary>
        [Required(ErrorMessage = "Residential status is required.")]
        [MaxLength(50, ErrorMessage = "Residential status cannot exceed 50 characters.")]
        public string ResidentialStatus { get; set; } = null!;

        /// <summary>
        /// Gender of the applicant.
        /// </summary>
        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        /// <summary>
        /// Digital signature image (in byte array format).
        /// </summary>
        [Required(ErrorMessage = "Signature image is required.")]
        public byte[] SignatureImage { get; set; } = null!;

        /// <summary>
        /// ID proof image (in byte array format).
        /// </summary>
        [Required(ErrorMessage = "ID proof image is required.")]
        public byte[] IDProofImage { get; set; } = null!;

    }
}

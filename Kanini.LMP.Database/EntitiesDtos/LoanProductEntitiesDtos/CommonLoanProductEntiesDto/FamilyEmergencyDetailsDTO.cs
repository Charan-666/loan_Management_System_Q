using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto
{
    public class FamilyEmergencyDetailsDTO
    {
        // Primary Key (optional — used for update/view)
        public int FamilyEmergencyDetailsId { get; set; }

        // Foreign Key — Loan Application
        [Required(ErrorMessage = "Loan Application ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        // Foreign Key — User (Applicant)
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        // Emergency Contact Info
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Relationship is required.")]
        [MaxLength(50, ErrorMessage = "Relationship cannot exceed 50 characters.")]
        public string RelationshipWithApplicant { get; set; } = null!;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number.")]
        public string MobileNumber { get; set; } = null!; // changed from int → string

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; } = null!;
    }
}

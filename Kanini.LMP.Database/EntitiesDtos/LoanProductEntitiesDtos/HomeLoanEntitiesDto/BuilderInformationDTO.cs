using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.HomeLoanEntitiesDto
{
    public class BuilderInformationDTO
    {
        // Unique identifier for the builder information record
        public int BuilderInformationId { get; set; }

        // Reference to associated loan application
        [Required(ErrorMessage = "Loan Application Base ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        // Linked user (Applicant)
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        // Builder details
        [Required(ErrorMessage = "Builder Name is required.")]
        [MaxLength(100, ErrorMessage = "Builder Name cannot exceed 100 characters.")]
        public string BuilderName { get; set; } = null!;

        [Required(ErrorMessage = "Project Name is required.")]
        [MaxLength(100, ErrorMessage = "Project Name cannot exceed 100 characters.")]
        public string ProjectName { get; set; } = null!;

        [Required(ErrorMessage = "Builder Registration Number is required.")]
        [MaxLength(50, ErrorMessage = "Registration Number cannot exceed 50 characters.")]
        public string BuilderRegistrationNo { get; set; } = null!;

        [Required(ErrorMessage = "Contact Number is required.")]
        [Phone(ErrorMessage = "Please enter a valid contact number.")]
        [MaxLength(15, ErrorMessage = "Contact Number cannot exceed 15 digits.")]
        public string ContactNumber { get; set; } = null!;

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = null!;
    }
}

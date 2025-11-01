using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.PersonalLoanEntitiesDto
{
    public class EmploymentDetailsDTO
    {
        // Primary key
        public int EmploymentDetailsId { get; set; }

        // Linked user (FK)
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        // Employment information
        [Required(ErrorMessage = "Company name is required.")]
        [MaxLength(150, ErrorMessage = "Company name cannot exceed 150 characters.")]
        public string CompanyName { get; set; } = null!;

        [Required(ErrorMessage = "Designation is required.")]
        [MaxLength(100, ErrorMessage = "Designation cannot exceed 100 characters.")]
        public string Designation { get; set; } = null!;

        [Required(ErrorMessage = "Experience is required.")]
        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public int Experience { get; set; }

        [Required(ErrorMessage = "Email ID is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(100, ErrorMessage = "Email ID cannot exceed 100 characters.")]
        public string EmailId { get; set; } = null!;

        [Required(ErrorMessage = "Office address is required.")]
        [MaxLength(250, ErrorMessage = "Office address cannot exceed 250 characters.")]
        public string OfficeAddress { get; set; } = null!;
    }
}

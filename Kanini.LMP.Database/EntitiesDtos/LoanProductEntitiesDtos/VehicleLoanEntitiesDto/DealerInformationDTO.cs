using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.VehicleLoanEntitiesDto
{
    public class DealerInformationDTO
    {
        // Primary key
        public int DealerInformationId { get; set; }

        // Foreign key → Linked Loan Application Base
        [Required(ErrorMessage = "LoanApplicationBaseId is required.")]
        public int LoanApplicationBaseId { get; set; }

        // Foreign key → Linked User
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        // Dealer details
        [Required(ErrorMessage = "Dealer name is required.")]
        [MaxLength(150, ErrorMessage = "Dealer name cannot exceed 150 characters.")]
        public string DealerName { get; set; } = null!;

        [Required(ErrorMessage = "Dealer address is required.")]
        [MaxLength(250, ErrorMessage = "Dealer address cannot exceed 250 characters.")]
        public string DealerAddress { get; set; } = null!;

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(15, ErrorMessage = "Contact number cannot exceed 15 digits.")]
        public string ContactNumber { get; set; } = null!;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
        public string Email { get; set; } = null!;
    }

}

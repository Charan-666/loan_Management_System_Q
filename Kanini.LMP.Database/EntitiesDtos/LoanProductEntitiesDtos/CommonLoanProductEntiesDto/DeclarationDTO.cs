using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto
{
    public class DeclarationDTO
    {
        // Primary Key (optional, mostly for update scenarios)
        public int DeclarationId { get; set; }

        // Foreign key (handled internally if part of nested create)
        public int LoanApplicationBaseId { get; set; }

        // --- Declaration Fields ---
        [Required(ErrorMessage = "Declaration name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
        public decimal Amount { get; set; }  // Changed to decimal for precision

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Purpose is required.")]
        [MaxLength(250, ErrorMessage = "Purpose cannot exceed 250 characters.")]
        public string Purpose { get; set; } = null!;
    }
}

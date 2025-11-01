using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto
{
    public class LoanProductDTO
    {
        // --- Identification ---
        public int LoanProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Loan type is required.")]
        [MaxLength(50, ErrorMessage = "Loan type cannot exceed 50 characters.")]
        public string LoanType { get; set; } = null!; // e.g., Personal Loan, Home Loan

        // --- Loan Details ---
        [Required(ErrorMessage = "Minimum amount is required.")]
        [Range(1000, double.MaxValue, ErrorMessage = "Minimum amount must be at least 1000.")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "Maximum amount is required.")]
        [Range(1000, double.MaxValue, ErrorMessage = "Maximum amount must be at least 1000.")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "Interest rate is required.")]
        [Range(0.1, 100, ErrorMessage = "Interest rate must be between 0.1 and 100%.")]
        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "Loan tenure is required.")]
        [Range(6, 360, ErrorMessage = "Tenure must be between 6 and 360 months.")]
        public int TenureMonths { get; set; }

        // --- Eligibility ---
        [MaxLength(250, ErrorMessage = "Eligibility criteria cannot exceed 250 characters.")]
        public string? EligibilityCriteria { get; set; }

        [MaxLength(250, ErrorMessage = "Required documents description cannot exceed 250 characters.")]
        public string? RequiredDocuments { get; set; }

        // --- Additional Info ---
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

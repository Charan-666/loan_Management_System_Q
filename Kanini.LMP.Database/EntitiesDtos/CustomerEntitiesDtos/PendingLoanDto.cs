using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto
{
    public class PendingLoanDto
    {
        [Required]
        public int LoanApplicationId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string LoanType { get; set; } = null!; // e.g., Personal, Home, Car

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Loan amount must be greater than zero.")]
        public decimal RequestedAmount { get; set; }

        [Required]
        [Range(1, 360, ErrorMessage = "Tenure must be between 1 and 360 months.")]
        public int TenureMonths { get; set; }

        [Required]
        public DateTime AppliedDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string CurrentStatus { get; set; } = "Pending"; // Default for this DTO

        // Optional Fields
        public decimal? InterestRate { get; set; }
        public decimal? MonthlyInstallment { get; set; }

        // Optional computed field
        public DateTime? LastUpdated { get; set; }
    }
}

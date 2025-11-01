using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto
{
    public class LoanDetailsDTO
    {
        // --- Primary Key (mostly used for GET or UPDATE operations) ---
        public int LoanDetailsId { get; set; }

        // --- Foreign Keys ---
        [Required(ErrorMessage = "Loan Application Base ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        [Required(ErrorMessage = "Loan Application ID is required.")]
        public int LoanApplicationId { get; set; }

        // --- Loan Amount ---
        [Required(ErrorMessage = "Requested amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Requested amount must be greater than zero.")]
        [Display(Name = "Requested Amount (₹)")]
        public decimal RequestedAmount { get; set; }

        // --- Tenure ---
        [Required(ErrorMessage = "Tenure in months is required.")]
        [Range(1, 360, ErrorMessage = "Tenure must be between 1 and 360 months.")]
        [Display(Name = "Loan Tenure (Months)")]
        public int TenureMonths { get; set; }

        // --- Application Date ---
        [Required(ErrorMessage = "Applied date is required.")]
        [Display(Name = "Application Date")]
        public DateTime AppliedDate { get; set; }

        // --- Optional Fields ---
        [Range(0.1, 100, ErrorMessage = "Interest rate must be between 0.1% and 100%.")]
        [Display(Name = "Interest Rate (%)")]
        public decimal? InterestRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Monthly installment must be a positive value.")]
        [Display(Name = "Monthly Installment (₹)")]
        public decimal? MonthlyInstallment { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.HomeLoanEntitiesDto
{
    public class HomeLoanDetailsDTO
    {
        // Unique identifier for the home loan details record
        public int HomeLoanDetailsId { get; set; }

        // Reference to associated loan base application
        [Required(ErrorMessage = "Loan Application Base ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        // Reference to specific home loan application
        [Required(ErrorMessage = "Loan Application ID is required.")]
        public int LoanApplicationId { get; set; }

        // Financial details
        [Required(ErrorMessage = "Property cost is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Property cost must be greater than zero.")]
        public decimal PropertyCost { get; set; }

        [Required(ErrorMessage = "Down payment is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Down payment cannot be negative.")]
        public decimal DownPayment { get; set; }

        [Required(ErrorMessage = "Requested loan amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Requested loan amount must be greater than zero.")]
        public decimal RequestedLoanAmount { get; set; }

        [Required(ErrorMessage = "Tenure (in months) is required.")]
        [Range(1, 360, ErrorMessage = "Tenure must be between 1 and 360 months.")]
        public int TenureMonths { get; set; }

        [Range(0, 100, ErrorMessage = "Interest rate must be between 0 and 100%.")]
        public decimal? InterestRate { get; set; }

        [Required(ErrorMessage = "Applied date is required.")]
        public DateTime AppliedDate { get; set; }

        [Required(ErrorMessage = "Loan purpose is required.")]
        public int LoanPurpose { get; set; } // Enum represented as int (LoanPurposeHome)
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto
{
    public class EligibilityScoreDto
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "LoanProductId is required.")]
        public int LoanProductId { get; set; }

        [Display(Name = "Credit Score")]
        [Range(300, 900, ErrorMessage = "Credit score must be between 300 and 900.")]
        public int CreditScore { get; set; }

        [Display(Name = "Monthly Income (₹)")]
        public decimal MonthlyIncome { get; set; }

        [Display(Name = "Existing EMIs (₹)")]
        public decimal ExistingEMIAmount { get; set; }

        [Display(Name = "Debt to Income Ratio (%)")]
        [Range(0, 100)]
        public double DebtToIncomeRatio { get; set; }

        [Display(Name = "Employment Type")]
        public string? EmploymentType { get; set; }  // e.g., Salaried, Self-Employed

        [Display(Name = "Loan Eligibility Score")]
        [Range(0, 100)]
        public double EligibilityScore { get; set; }  // calculated overall score (0–100)

        [Display(Name = "Eligibility Status")]
        public string? EligibilityStatus { get; set; } // e.g., "Eligible", "Not Eligible", "Needs Review"

        [Display(Name = "Calculated Date")]
        public DateTime CalculatedOn { get; set; } = DateTime.UtcNow;
    }
}

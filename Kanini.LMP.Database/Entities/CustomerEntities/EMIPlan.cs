using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class EMIPlan
    {
        [Key]
        public int EMIId { get; set; }
        // FK → Linked Personal Loan Application
        [Required]
        public int LoanApplicationBaseId { get; set; }

        public int LoanAccountId { get; set; }

        // Principal Loan Amount
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Principle amount must be greater than zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrincipleAmount { get; set; }
        // Loan Term in Months (1 to 360)
        [Required]
        [Range(1, 360, ErrorMessage = "Term must be between 1 and 360 months")]
        public int TermMonths { get; set; }
        // Rate of Interest (Annual)
        [Required]
        [Range(0.1, 100, ErrorMessage = "Interest rate must be valid")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal RateOfInterest { get; set; }
        // Calculated Monthly EMI
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyEMI { get; set; }

        // Total Interest Paid Across Entire Loan
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalInterestPaid { get; set; }
        // Total Amount to Repay (Principal + Interest)
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRepaymentAmount { get; set; }
        // EMI Plan State (Active, Closed, Defaulted, etc.)
        [Required]
        public EMIPlanStatus Status { get; set; } = EMIPlanStatus.Active;
        // Indicates whether the repayment is fully completed
        [Required]
        public bool IsCompleted { get; set; }

    }
}

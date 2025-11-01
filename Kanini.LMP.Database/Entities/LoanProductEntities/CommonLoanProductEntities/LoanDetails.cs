using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class LoanDetails
    {
        [Key]
        public int LoanDetailsId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // FK → Linked Loan Application
        [Required]
        [ForeignKey(nameof(PersonalLoanApplication))]
        public int LoanApplicationId { get; set; }
        // Requested loan amount
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Requested amount must be greater than zero")]
        public decimal RequestedAmount { get; set; }
        // Loan tenure in months
        [Required]
        [Range(1, 360, ErrorMessage = "Tenure must be between 1 and 360 months")]
        public int TenureMonths { get; set; }
        // Date when application was submitted
        [Required]

        public DateTime AppliedDate { get; set; }
        // Optional: Interest rate and monthly installment
        [Column(TypeName = "decimal(5,2)")]
        public decimal? InterestRate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyInstallment { get; set; }
    }
}

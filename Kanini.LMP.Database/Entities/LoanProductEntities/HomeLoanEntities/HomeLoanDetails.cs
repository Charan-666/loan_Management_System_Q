using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities
{
    public class HomeLoanDetails
    {
        [Key]
        public int HomeLoanDetailsId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // FK → Linked Loan Application
        [Required]
        [ForeignKey(nameof(PersonalLoanApplication))]
        public int LoanApplicationId { get; set; }
        // Financial details
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Property cost must be greater than zero")]

        public decimal PropertyCost { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Down payment cannot be negative")]
        public decimal DownPayment { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Requested loan amount must be greater than zero")]
        public decimal RequestedLoanAmount { get; set; }
        [Required]
        [Range(1, 360, ErrorMessage = "Tenure must be between 1 and 360 months")]
        public int TenureMonths { get; set; }
        // Optional interest rate
        [Column(TypeName = "decimal(5,2)")]
        public decimal? InterestRate { get; set; }
        // Date of loan application
        [Required]
        public DateTime AppliedDate { get; set; }
        // Purpose of the home loan
        [Required]
        public LoanPurposeHome LoanPurpose { get; set; } 

    }
}

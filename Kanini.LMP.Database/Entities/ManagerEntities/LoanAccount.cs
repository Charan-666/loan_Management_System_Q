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

namespace Kanini.LMP.Database.Entities.ManagerEntities
{
    public class LoanAccount
    {
       
            [Key]
            public int LoanAccountId { get; set; }

            // LINK CHANGE: Reference the base class ID, not a specific product ID.
            // This allows a Home Loan, Personal Loan, or Vehicle Loan to link here.
            [ForeignKey(nameof(LoanApplicationBase))]
            public int LoanApplicationBaseId { get; set; } // Renamed from LoanApplicationId

            // Link to the Customer (Note: This is now optional/redundant if you rely on the M:M LoanApplicant table)
            [ForeignKey(nameof(Customer))]
            public int CustomerId { get; set; }

         
            [Required]
            public LoanPaymentStatus CurrentPaymentStatus { get; set; }

            public DateTime DisbursementDate { get; set; }

            public int DaysPastDue { get; set; } = 0;

            public DateTime LastStatusUpdate { get; set; }

           
            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal TotalLoanAmount { get; set; }
            [Column(TypeName = "decimal(18,2)")]

            public decimal TotalPaidPrincipal { get; set; } = 0m;
            [Column(TypeName = "decimal(18,2)")]
            public decimal TotalPaidInterest { get; set; } = 0m;
            [Column(TypeName = "decimal(18,2)")]

            public decimal PrincipalRemaining { get; set; }

            public DateTime? LastPaymentDate { get; set; }
            [Column(TypeName = "decimal(18,2)")]

            public decimal TotalLateFeePaidAmount { get; set; } = 0m;
        }
    }


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.CustomerScape
{
    public class CustomerLoanSummaryDto
    {

        /// <summary>
        /// DTO summarizing a single loan application and its current servicing status.
        /// This structure is used for the expandable list of loans (Vehicle Loan, Personal Loan, etc.).
        /// </summary>




        // Application Details (From LoanApplication and LoanDetails)
        public int LoanApplicationId { get; set; }
        public string LoanProductType { get; set; } = null!;
        public decimal LoanAmountRequested { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public int LoanTermMonths { get; set; } // TenureMonths

        // Servicing Details (From your LoanAccount model)
        public int? LoanAccountId { get; set; } // Null if not yet approved/disbursed
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalPaidPrincipal { get; set; }
        public decimal TotalPaidInterest { get; set; }
        public decimal RemainingPrincipal { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal LastPaidAmount { get; set; }
        public DateTime? NextPaymentDate { get; set; } // Derived from payment schedule
        public decimal RemainingPaymentDue { get; set; }
        public decimal TotalLateFeePaidAmount { get; set; }

    }
}
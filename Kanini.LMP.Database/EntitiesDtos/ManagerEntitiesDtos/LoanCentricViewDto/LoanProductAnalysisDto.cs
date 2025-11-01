using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanCentricView
{
    internal class LoanProductAnalysisDto
    {
        /// <summary>
        /// DTO to populate the 9 metric cards on the detail analysis page.
        /// ALWAYS populated after filtering ALL data by a single LoanProductType (e.g., "Consumer Loan").
        /// </summary>



        public string ApplicationType { get; set; } 
        public int TotalNoOfLoans { get; set; }


        public decimal AvgLoanAmount { get; set; }

        public decimal AvgInterestRate { get; set; }

    
        public decimal AvgMonthlyInstallment { get; set; }

        public decimal LoanSuccessRate { get; set; }
        public decimal CurrentLoanRate { get; set; }
        public decimal LoanPendingRate { get; set; }

        public decimal CustomerAvgAnnualIncome { get; set; }

        public string LoanSegmentationKeyFinding { get; set; } = "N/A";

    }
}

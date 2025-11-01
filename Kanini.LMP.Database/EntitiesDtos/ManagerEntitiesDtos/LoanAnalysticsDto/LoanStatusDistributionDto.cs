using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics
{
    internal class LoanStatusDistributionDto
    {
        /// <summary>
        /// DTO for the 'Loan Status Distribution' chart (Bar with percentages).
        /// Source: LoanAccount (group by CurrentPaymentStatus)
        /// </summary>
        public string StatusName { get; set; } // e.g., Fully Paid, Late (> 1-100 Days), Default
        public decimal Percentage { get; set; }
    }
}

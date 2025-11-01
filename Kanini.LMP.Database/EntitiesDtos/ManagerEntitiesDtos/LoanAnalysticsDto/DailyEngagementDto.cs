
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics
{
    public class DailyEngagementDto
    {

        /// <summary>
        /// DTO for the 'Loan Engagement' chart (Daily Bar Chart).
        /// Source: LoanApplication (group by ApprovedDate/DisbursementDate)
        /// </summary>
        public int DayOfMonth { get; set; } // 8, 9, 10...
        public int NumberOfLoans { get; set; } // Count of loans approved/issued that day
    }
}
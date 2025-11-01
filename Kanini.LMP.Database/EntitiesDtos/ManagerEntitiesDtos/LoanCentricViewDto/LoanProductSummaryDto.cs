
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanCentricView
{
    public class LoanProductSummaryDto
    {
        public string LoanProductType { get; set; } 

        /// <summary>
        /// Percentage of applications for this type that reached 'Approved' status.
        /// Logic: COUNT(Approved) / COUNT(Total Submissions)
        /// </summary>
        public decimal SuccessRatePercentage { get; set; }

    }
}

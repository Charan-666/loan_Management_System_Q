
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics
{
    public class LoanTypeDistributionDto
    {

        /// <summary>
        /// DTO for the 'Loan Type Distribution' chart (Donut Chart).
        /// Source: LoanApplication (group by LoanProductType)
        /// </summary>
        public string LoanTypeName { get; set; } 
        public decimal Percentage { get; set; } // The percentage of the total
    }
}
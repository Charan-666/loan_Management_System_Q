using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics
{
    public class CountryDistributionDto
    {

        /// <summary>
        /// DTO for the 'Loan Issued Country Distribution' chart (Pie Chart).
        /// Source: LoanApplication -> AddressInformation (uses Country field)
        /// </summary>
        public string Country { get; set; } 
        public decimal Percentage { get; set; } // Percentage of loans issued in this country
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics
{
    public class CountryLoanTypeVolumeDto
    {

        /// <summary>
        /// DTO for the 'Issued Country vs Loan Type' chart (Stacked/Grouped Bar Chart).
        /// Source: LoanApplication -> AddressInformation (Country)
        /// </summary>
        public string LoanTypeName { get; set; }
       
        // Key: Country (e.g., "US"), Value: Volume (e.g., 3000)
        public Dictionary<string, int> CountryVolumes { get; set; } = new Dictionary<string, int>();
        public int TotalVolume { get; set; } 
    }
}
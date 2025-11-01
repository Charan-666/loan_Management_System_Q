using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication
{
    public class VehicleValuationDTO
    {
        public decimal MarketValue { get; set; }
        public decimal DealerQuotedValue { get; set; }
        public decimal AssessedValue { get; set; }
        public string AssessedBy { get; set; } = string.Empty;
        public DateOnly ValuationDate { get; set; }
    }
}

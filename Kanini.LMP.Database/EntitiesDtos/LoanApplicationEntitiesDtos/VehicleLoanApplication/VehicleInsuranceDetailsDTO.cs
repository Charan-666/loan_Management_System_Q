using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication
{
    public class VehicleInsuranceDetailsDTO
    {
        public string InsuranceProvider { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public DateOnly PolicyStartDate { get; set; }
        public DateOnly PolicyEndDate { get; set; }
        public decimal PremiumAmount { get; set; }
    }
}

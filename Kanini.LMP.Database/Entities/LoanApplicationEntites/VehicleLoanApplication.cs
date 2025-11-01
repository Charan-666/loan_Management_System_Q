using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class VehicleLoanApplication : LoanApplicationBase
    {
        // Product-specific details for Vehicle Loans:
        [Required]
        public DealerInformation DealerInformation { get; set; } = null!;
        [Required]
        public VehicleLoanDetails VehicleLoanDetails { get; set; } = null!;
        [Required]
        public VehicleInformation VehicleInformation { get; set; } = null!;
    }
}

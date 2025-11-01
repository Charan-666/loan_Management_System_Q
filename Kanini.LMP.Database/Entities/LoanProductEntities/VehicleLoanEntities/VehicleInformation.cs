using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities
{
    public class VehicleInformation
    {
        [Key]
        public int VehicleInformationId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // FK → Linked User
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Vehicle characteristics
        [Required]

        public VehicleType VehicleType { get; set; }  // Car, Bike, SUV, etc.
        [Required]
        [MaxLength(100)]
        public string Manufacturer { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Variant { get; set; } = null!;
        [Required]
        [Range(1900, 2100, ErrorMessage = "Enter a valid manufacturing year")]
        public int ManufacturingYear { get; set; }
        [Required]
        public LoanPurposeVehicle VehicleCondition { get; set; } // New or Used
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Ex-showroom price must be greater than zero")]
        public decimal ExShowroomPrice { get; set; }
    }

}

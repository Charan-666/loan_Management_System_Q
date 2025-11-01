using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.VehicleLoanEntitiesDto
{
    public class VehicleInformationDTO
    {
        // Primary Key
        public int VehicleInformationId { get; set; }

        // Foreign Key → Loan Application
        [Required(ErrorMessage = "LoanApplicationBaseId is required.")]
        public int LoanApplicationBaseId { get; set; }

        // Foreign Key → Linked User
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        // Vehicle characteristics
        [Required(ErrorMessage = "Vehicle type is required.")]
        public VehicleType VehicleType { get; set; } // Enum: Car, Bike, SUV, etc.

        [Required(ErrorMessage = "Manufacturer is required.")]
        [MaxLength(100, ErrorMessage = "Manufacturer name cannot exceed 100 characters.")]
        public string Manufacturer { get; set; } = null!;

        [Required(ErrorMessage = "Model is required.")]
        [MaxLength(100, ErrorMessage = "Model name cannot exceed 100 characters.")]
        public string Model { get; set; } = null!;

        [Required(ErrorMessage = "Variant is required.")]
        [MaxLength(100, ErrorMessage = "Variant name cannot exceed 100 characters.")]
        public string Variant { get; set; } = null!;

        [Required(ErrorMessage = "Manufacturing year is required.")]
        [Range(1900, 2100, ErrorMessage = "Enter a valid manufacturing year between 1900 and 2100.")]
        public int ManufacturingYear { get; set; }

        [Required(ErrorMessage = "Vehicle condition is required.")]
        public LoanPurposeVehicle VehicleCondition { get; set; } // Enum: New or Used

        [Required(ErrorMessage = "Ex-showroom price is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Ex-showroom price must be greater than zero.")]
        public decimal ExShowroomPrice { get; set; }
    }
}

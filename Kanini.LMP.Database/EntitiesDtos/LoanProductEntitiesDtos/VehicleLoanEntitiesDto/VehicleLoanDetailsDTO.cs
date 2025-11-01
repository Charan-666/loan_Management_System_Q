using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.VehicleLoanEntitiesDto
{
    public class VehicleLoanDetailsDTO
    {
        // Primary Key
        public int VehicleLoanDetailsId { get; set; }

        // Foreign Key → Base Loan Application
        [Required(ErrorMessage = "LoanApplicationBaseId is required.")]
        public int LoanApplicationBaseId { get; set; }

        // Foreign Key → Linked Loan Application (Vehicle/Personal)
        [Required(ErrorMessage = "LoanApplicationId is required.")]
        public int LoanApplicationId { get; set; }

        // Financial details
        [Required(ErrorMessage = "On-road price is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "On-road price must be greater than zero.")]
        public decimal OnRoadPrice { get; set; }

        [Required(ErrorMessage = "Down payment is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Down payment cannot be negative.")]
        public decimal DownPayment { get; set; }

        [Required(ErrorMessage = "Requested loan amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Requested loan amount must be greater than zero.")]
        public decimal RequestedLoanAmount { get; set; }

        [Required(ErrorMessage = "Tenure (in months) is required.")]
        [Range(1, 360, ErrorMessage = "Tenure must be between 1 and 360 months.")]
        public int TenureMonths { get; set; }

        // Optional interest rate
        [Range(0, 100, ErrorMessage = "Interest rate must be between 0 and 100%.")]
        public decimal? InterestRate { get; set; }

        // Date of loan application
        [Required(ErrorMessage = "Applied date is required.")]
        public DateTime AppliedDate { get; set; }

        // Purpose of the vehicle loan (Enum)
        [Required(ErrorMessage = "Loan purpose is required.")]
        public LoanPurposeVehicle LoanPurposeVehicle { get; set; }
    }
}

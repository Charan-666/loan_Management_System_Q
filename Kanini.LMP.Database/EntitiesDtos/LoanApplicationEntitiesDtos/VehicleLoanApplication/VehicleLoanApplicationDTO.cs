using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.PersonalLoanEntitiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.VehicleLoanEntitiesDto;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication
{
    public class VehicleLoanApplicationDTO
    {
        // Primary Key
        public int LoanApplicationBaseId { get; set; }

        // Loan Type
        [Required(ErrorMessage = "Loan product type is required.")]
        public string LoanProductType { get; set; } = null!;

        // Common Nested Details (from LoanApplicationBase)
        [Required(ErrorMessage = "Loan details are required.")]
        public LoanDetailsDTO LoanDetails { get; set; } = null!;

        [Required(ErrorMessage = "Personal details are required.")]
        public PersonalDetailsDTO PersonalDetails { get; set; } = null!;

        [Required(ErrorMessage = "Address information is required.")]
        public AddressInformationDTO AddressInformation { get; set; } = null!;

        [Required(ErrorMessage = "Family emergency details are required.")]
        public FamilyEmergencyDetailsDTO FamilyEmergencyDetails { get; set; } = null!;

        [Required(ErrorMessage = "Employment details are required.")]
        public EmploymentDetailsDTO EmploymentDetails { get; set; } = null!;

        [Required(ErrorMessage = "Financial information is required.")]
        public FinancialInformationDTO FinancialInformation { get; set; } = null!;

        [Required(ErrorMessage = "Declaration is required.")]
        public DeclarationDTO Declaration { get; set; } = null!;

        // Common Workflow Fields
        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;

        public DateOnly SubmissionDate { get; set; }

        public DateOnly? ApprovedDate { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Common Collections (IDs only in DTOs)
        public ICollection<int>? ApplicantIds { get; set; } = new List<int>();
        public ICollection<int>? DocumentLinkIds { get; set; } = new List<int>();

        // Vehicle Loan–specific details
        [Required(ErrorMessage = "Dealer information is required.")]
        public DealerInformationDTO DealerInformation { get; set; } = null!;

        [Required(ErrorMessage = "Vehicle loan details are required.")]
        public VehicleLoanDetailsDTO VehicleLoanDetails { get; set; } = null!;

        [Required(ErrorMessage = "Vehicle information is required.")]
        public VehicleInformationDTO VehicleInformation { get; set; } = null!;
    }
}

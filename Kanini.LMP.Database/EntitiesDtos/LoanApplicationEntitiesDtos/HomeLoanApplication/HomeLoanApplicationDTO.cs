using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.HomeLoanEntitiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.PersonalLoanEntitiesDto;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication
{
    public class HomeLoanApplicationDTO
    {
        //  Primary Identifier
        public int LoanApplicationBaseId { get; set; }

        //  Type of loan (discriminator)
        [Required]
        [MaxLength(100)]
        public string LoanProductType { get; set; } = null!;

        //  Common Application Info
        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;

        [Required]
        public DateOnly SubmissionDate { get; set; }

        public DateOnly? ApprovedDate { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        //  Builder Information (Nested Object)
        [Required]
        public BuilderInformationDTO BuilderInformation { get; set; } = null!;

        //  Home Loan Details
        [Required]
        public HomeLoanDetailsDTO HomeLoanDetails { get; set; } = null!;

        //  Property Details
        [Required]
        public PropertyDetailsDTO PropertyDetails { get; set; } = null!;

        //  Loan Details (Common Loan Information)
        [Required]
        public LoanDetailsDTO LoanDetails { get; set; } = null!;

        //  Personal Details
        [Required]
        public PersonalDetailsDTO PersonalDetails { get; set; } = null!;

        //  Address Information
        [Required]
        public AddressInformationDTO AddressInformation { get; set; } = null!;

        //  Family Emergency
        [Required]
        public FamilyEmergencyDetailsDTO FamilyEmergencyDetails { get; set; } = null!;

        //  Employment Info
        [Required]
        public EmploymentDetailsDTO EmploymentDetails { get; set; } = null!;

        //  Financial Info
        [Required]
        public FinancialInformationDTO FinancialInformation { get; set; } = null!;

        //  Declaration Info
        [Required]
        public DeclarationDTO Declaration { get; set; } = null!;

        //  Applicants (M:M relation → send only IDs)
        public ICollection<int>? ApplicantIds { get; set; }

        //  Documents Linked
        public ICollection<int>? DocumentLinkIds { get; set; }
    }
}

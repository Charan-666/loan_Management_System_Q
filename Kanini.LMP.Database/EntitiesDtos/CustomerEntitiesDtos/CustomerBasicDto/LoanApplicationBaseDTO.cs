using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.PersonalLoanEntitiesDto;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto
{
    public class LoanApplicationBaseDTO
    {
        public int LoanApplicationBaseId { get; set; }

        [Required]
        public string LoanProductType { get; set; } = null!;

        // --- Common Nested DTOs ---
        [Required]
        public LoanDetailsDTO LoanDetails { get; set; } = null!;

        [Required]
        public PersonalDetailsDTO PersonalDetails { get; set; } = null!;

        [Required]
        public AddressInformationDTO AddressInformation { get; set; } = null!;

        [Required]
        public FamilyEmergencyDetailsDTO FamilyEmergencyDetails { get; set; } = null!;

        [Required]
        public EmploymentDetailsDTO EmploymentDetails { get; set; } = null!;

        [Required]
        public FinancialInformationDTO FinancialInformation { get; set; } = null!;

        [Required]
        public DeclarationDTO Declaration { get; set; } = null!;

        // --- Workflow Fields ---
        public ApplicationStatus Status { get; set; }

        public DateOnly SubmissionDate { get; set; }

        public DateOnly? ApprovedDate { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        public bool IsActive { get; set; }

        // --- Related Collections ---
        public ICollection<int>? ApplicantIds { get; set; }
        public ICollection<int>? DocumentIds { get; set; }
    }
}

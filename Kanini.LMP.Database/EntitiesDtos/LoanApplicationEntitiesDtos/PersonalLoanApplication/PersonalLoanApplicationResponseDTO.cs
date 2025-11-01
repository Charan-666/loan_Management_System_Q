using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.PersonalLoanEntitiesDto;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication
{
    public class PersonalLoanApplicationResponseDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public string LoanProductType { get; set; } = "PersonalLoan";
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }

        // 🔹 Detailed Data
        public LoanDetailsDTO LoanDetails { get; set; } = null!;
        public PersonalDetailsDTO PersonalDetails { get; set; } = null!;
        public AddressInformationDTO AddressInformation { get; set; } = null!;
        public FamilyEmergencyDetailsDTO FamilyEmergencyDetails { get; set; } = null!;
        public EmploymentDetailsDTO EmploymentDetails { get; set; } = null!;
        public FinancialInformationDTO FinancialInformation { get; set; } = null!;
        public DeclarationDTO Declaration { get; set; } = null!;

        // 🔹 Linked Entities
        public ICollection<int>? ApplicantIds { get; set; }
        public ICollection<int>? DocumentLinkIds { get; set; }
    }
}

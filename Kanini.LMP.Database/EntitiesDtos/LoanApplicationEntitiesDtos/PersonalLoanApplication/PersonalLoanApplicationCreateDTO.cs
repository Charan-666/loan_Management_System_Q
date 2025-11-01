using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.PersonalLoanEntitiesDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication
{
    public class PersonalLoanApplicationCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string LoanProductType { get; set; } = "PersonalLoan";

        [Required]
        public DateOnly SubmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        // 🔹 Nested DTOs required for input
        [Required] public LoanDetailsDTO LoanDetails { get; set; } = null!;
        [Required] public PersonalDetailsDTO PersonalDetails { get; set; } = null!;
        [Required] public AddressInformationDTO AddressInformation { get; set; } = null!;
        [Required] public FamilyEmergencyDetailsDTO FamilyEmergencyDetails { get; set; } = null!;
        [Required] public EmploymentDetailsDTO EmploymentDetails { get; set; } = null!;
        [Required] public FinancialInformationDTO FinancialInformation { get; set; } = null!;
        [Required] public DeclarationDTO Declaration { get; set; } = null!;

        // 🔹 Optional linked IDs
        public ICollection<int>? ApplicantIds { get; set; }
        public ICollection<int>? DocumentLinkIds { get; set; }
    }
}

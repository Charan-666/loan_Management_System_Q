using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails
{
    public class PersonalDetailsResponseDTO
    {
        public int PersonalDetailsId { get; set; }

        public int LoanApplicationBaseId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public DateOnly DateOfBirth { get; set; }

        public string DistrictOfBirth { get; set; } = null!;

        public string CountryOfBirth { get; set; } = null!;

        public string PANNumber { get; set; } = null!;

        public string EducationQualification { get; set; } = null!;

        public string ResidentialStatus { get; set; } = null!;

        public Gender Gender { get; set; }

        public byte[] SignatureImage { get; set; } = null!;

        public byte[] IDProofImage { get; set; } = null!;
    }
}

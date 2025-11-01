using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails
{
    public class PersonalDetailsUpdateDTO
    {
        [Required(ErrorMessage = "PersonalDetailsId is required for updating.")]
        public int PersonalDetailsId { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? DistrictOfBirth { get; set; }

        [MaxLength(100)]
        public string? CountryOfBirth { get; set; }

        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN number format.")]
        [MaxLength(10)]
        public string? PANNumber { get; set; }

        [MaxLength(100)]
        public string? EducationQualification { get; set; }

        [MaxLength(50)]
        public string? ResidentialStatus { get; set; }

        public Gender? Gender { get; set; }

        public byte[]? SignatureImage { get; set; }

        public byte[]? IDProofImage { get; set; }
    }
}

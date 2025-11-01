using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class PersonalDetails
    {
        [Key]
        public int PersonalDetailsId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }
        // FK → Linked User
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Personal info
        [Required]
        [MaxLength(100)]

        public string FullName { get; set; } = null!;
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        [MaxLength(100)]
        public string DistrictOfBirth { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string CountryOfBirth { get; set; } = null!;
        [Required]
        [MaxLength(10)]
        public string PANNumber { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string EducationQualification { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string ResidentialStatus { get; set; } = null!;
        [Required]
        public Gender Gender { get; set; }
        // Digital images
        [Required]
        [Column(TypeName = "varbinary(max)")]

        public byte[] SignatureImage { get; set; } = null!;
        [Required]
        [Column(TypeName = "varbinary(max)")]
        public byte[] IDProofImage { get; set; } = null!;

    }
}
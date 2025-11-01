using Kanini.LMP.Database.Entities.CustomerEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities
{
    public class BuilderInformation
    {
        [Key]
        public int BuilderInformationId { get; set; } 

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // FK → Linked User
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Builder details
        [Required]
        [MaxLength(100)]

        public string BuilderName { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string ProjectName { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string BuilderRegistrationNo { get; set; } = null!;
        [Required]
        [Phone]
        [MaxLength(15)]
        public string ContactNumber { get; set; } = null!;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

    }
}

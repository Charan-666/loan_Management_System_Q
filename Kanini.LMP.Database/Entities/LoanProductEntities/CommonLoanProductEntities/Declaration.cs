using Kanini.LMP.Database.Entities.CustomerEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class Declaration
    {
        [Key]
        public int DeclarationId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // Name of the declaration
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        // Declared amount
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be non-negative")]
        [Column(TypeName = "decimal(18,2)")]
        public int Amount { get; set; }
        // Description of the declaration
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;
        // Purpose or reason for declaration
        [Required]
        [MaxLength(250)]
        public string Purpose { get; set; } = null!;

    }
}

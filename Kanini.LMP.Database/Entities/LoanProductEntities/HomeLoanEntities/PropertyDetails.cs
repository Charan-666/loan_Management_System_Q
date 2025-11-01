using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities
{
    public class PropertyDetails
    {
        [Key]
        public int PropertyDetailsId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // FK → Linked User
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Property characteristics
        [Required]
        public PropertyType PropertyType { get; set; }
        [Required]
        [MaxLength(250)]
        public string PropertyAddress { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string State { get; set; } = null!;
        [Required]
        public int ZipCode { get; set; }
        [Required]
        public OwnershipType OwnershipType { get; set; }

    }
}

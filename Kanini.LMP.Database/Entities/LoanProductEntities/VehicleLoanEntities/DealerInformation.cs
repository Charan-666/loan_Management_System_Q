using Kanini.LMP.Database.Entities.CustomerEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities
{
    public class DealerInformation
    {
        [Key]
        public int DealerInformationId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // FK → Linked User
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Dealer details
        [Required]
        [MaxLength(150)]

        public string DealerName { get; set; } = null!;
        [Required]
        [MaxLength(250)]
        public string DealerAddress { get; set; } = null!;
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

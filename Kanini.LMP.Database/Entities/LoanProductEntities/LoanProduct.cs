using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.Entities.LoanProductEntities
{
    public class LoanProduct
    {
        [Key]
        public int LoanProductId { get; set; } // Primary Key
        [Required]
        [MaxLength(100)]
        public string LoanProductName { get; set; } = null!;// Name of the loan product
        [Required]
        [MaxLength(500)]
        public string LoanProductDescription { get; set; } = null!; // Description of the loan product
        [Required]
        public bool IsActive { get; set; } = true; //status of the loan product
    }
}

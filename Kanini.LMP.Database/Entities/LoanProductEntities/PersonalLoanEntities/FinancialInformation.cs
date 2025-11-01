using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities
{
    public class FinancialInformation
    {
        [Key]
        public int FinancialInformationId { get; set; }
        // FK → Linked User
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Income and expense details (all required)
        [Required]
        [Range(0, int.MaxValue)]
        public int Salary { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Rent { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int PrimaryOther { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int RentandUtility { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int FoodandClothing { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Education { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int LoanRepayment { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int ExpenseOther { get; set; }

    }
}
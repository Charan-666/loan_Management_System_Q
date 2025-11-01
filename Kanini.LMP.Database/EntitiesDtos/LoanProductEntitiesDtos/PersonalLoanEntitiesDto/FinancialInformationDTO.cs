using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.PersonalLoanEntitiesDto
{
    public class FinancialInformationDTO
    {
        // Primary key
        public int FinancialInformationId { get; set; }

        // Foreign key → Linked user
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        // Income and expense details (all required)
        [Required(ErrorMessage = "Salary is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Salary cannot be negative.")]
        public int Salary { get; set; }

        [Required(ErrorMessage = "Rent is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Rent cannot be negative.")]
        public int Rent { get; set; }

        [Required(ErrorMessage = "Primary other income is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Primary other income cannot be negative.")]
        public int PrimaryOther { get; set; }

        [Required(ErrorMessage = "Rent and utility expense is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Rent and utility cannot be negative.")]
        public int RentandUtility { get; set; }

        [Required(ErrorMessage = "Food and clothing expense is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Food and clothing expense cannot be negative.")]
        public int FoodandClothing { get; set; }

        [Required(ErrorMessage = "Education expense is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Education expense cannot be negative.")]
        public int Education { get; set; }

        [Required(ErrorMessage = "Loan repayment is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Loan repayment cannot be negative.")]
        public int LoanRepayment { get; set; }

        [Required(ErrorMessage = "Other expenses are required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Other expenses cannot be negative.")]
        public int ExpenseOther { get; set; }
    }
}

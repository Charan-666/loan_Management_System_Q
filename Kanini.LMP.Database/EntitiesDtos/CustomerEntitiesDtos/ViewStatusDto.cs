using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto
{
    public class ViewStatusDto
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Display(Name = "Customer Name")]
        public string? CustomerName { get; set; }

        // List of loan application statuses
        public List<LoanStatusInfoDto> Applications { get; set; } = new();
    }
    // Nested DTO to represent each loan application's status summary
    public class LoanStatusInfoDto
    {
        public int LoanApplicationId { get; set; }

        [Display(Name = "Loan Type")]
        public string? LoanType { get; set; }

        [Display(Name = "Requested Amount (₹)")]
        public decimal RequestedAmount { get; set; }

        [Display(Name = "Applied Date")]
        public DateTime AppliedDate { get; set; }

        [Display(Name = "Application Status")]
        public LoanStatus Status { get; set; }

        [Display(Name = "Remarks (if any)")]
        public string? Remarks { get; set; }
    }
}

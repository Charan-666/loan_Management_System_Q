using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos
{
    public class CustomerEMIDashboardDto
    {
        public int EMIId { get; set; }
        public int LoanAccountId { get; set; }
        
        [Display(Name = "Total Loan Amount")]
        public decimal TotalLoanAmount { get; set; }
        
        [Display(Name = "Monthly EMI")]
        public decimal MonthlyEMI { get; set; }
        
        [Display(Name = "Pending Amount")]
        public decimal PendingAmount { get; set; }
        
        [Display(Name = "Total Interest")]
        public decimal TotalInterest { get; set; }
        
        [Display(Name = "Interest Paid")]
        public decimal InterestPaid { get; set; }
        
        [Display(Name = "Principal Paid")]
        public decimal PrincipalPaid { get; set; }
        
        [Display(Name = "Current Month EMI")]
        public decimal CurrentMonthEMI { get; set; }
        
        [Display(Name = "Next Due Date")]
        public DateTime? NextDueDate { get; set; }
        
        [Display(Name = "EMIs Paid")]
        public int EMIsPaid { get; set; }
        
        [Display(Name = "EMIs Remaining")]
        public int EMIsRemaining { get; set; }
        
        [Display(Name = "Loan Status")]
        public string Status { get; set; } = string.Empty;
        
        [Display(Name = "Is Overdue")]
        public bool IsOverdue { get; set; }
        
        [Display(Name = "Days Overdue")]
        public int DaysOverdue { get; set; }
        
        [Display(Name = "Late Fee Amount")]
        public decimal LateFeeAmount { get; set; }
        
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
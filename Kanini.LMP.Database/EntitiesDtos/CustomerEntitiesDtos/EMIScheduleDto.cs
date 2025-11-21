using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos
{
    public class EMIScheduleDto
    {
        public int InstallmentNumber { get; set; }
        
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        
        [Display(Name = "EMI Amount")]
        public decimal EMIAmount { get; set; }
        
        [Display(Name = "Principal Component")]
        public decimal PrincipalAmount { get; set; }
        
        [Display(Name = "Interest Component")]
        public decimal InterestAmount { get; set; }
        
        [Display(Name = "Outstanding Balance")]
        public decimal OutstandingBalance { get; set; }
        
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = "Pending";
        
        [Display(Name = "Paid Date")]
        public DateTime? PaidDate { get; set; }
        
        [Display(Name = "Late Fee")]
        public decimal LateFee { get; set; }
    }

    public class PrepaymentCalculationDto
    {
        [Display(Name = "Current Outstanding")]
        public decimal CurrentOutstanding { get; set; }
        
        [Display(Name = "Prepayment Amount")]
        public decimal PrepaymentAmount { get; set; }
        
        [Display(Name = "New Outstanding")]
        public decimal NewOutstanding { get; set; }
        
        [Display(Name = "Interest Saved")]
        public decimal InterestSaved { get; set; }
        
        [Display(Name = "New EMI Amount")]
        public decimal NewEMIAmount { get; set; }
        
        [Display(Name = "Reduced Tenure (Months)")]
        public int ReducedTenure { get; set; }
        
        [Display(Name = "Prepayment Charges")]
        public decimal PrepaymentCharges { get; set; }
        
        [Display(Name = "Net Savings")]
        public decimal NetSavings { get; set; }
    }

    public class EMIRestructureDto
    {
        [Required]
        public int EMIId { get; set; }
        
        [Display(Name = "New Tenure (Months)")]
        [Range(1, 360)]
        public int? NewTenureMonths { get; set; }
        
        [Display(Name = "New Interest Rate (%)")]
        [Range(1, 50)]
        public decimal? NewInterestRate { get; set; }
        
        [Display(Name = "Moratorium Period (Months)")]
        [Range(0, 12)]
        public int MoratoriumMonths { get; set; }
        
        [Display(Name = "Restructure Reason")]
        public string RestructureReason { get; set; } = string.Empty;
    }

    public class EMIRestructureResultDto
    {
        [Display(Name = "Original EMI")]
        public decimal OriginalEMI { get; set; }
        
        [Display(Name = "New EMI")]
        public decimal NewEMI { get; set; }
        
        [Display(Name = "Original Tenure")]
        public int OriginalTenure { get; set; }
        
        [Display(Name = "New Tenure")]
        public int NewTenure { get; set; }
        
        [Display(Name = "Additional Interest")]
        public decimal AdditionalInterest { get; set; }
        
        [Display(Name = "Restructure Charges")]
        public decimal RestructureCharges { get; set; }
        
        [Display(Name = "New Schedule")]
        public List<EMIScheduleDto> NewSchedule { get; set; } = new();
    }
}
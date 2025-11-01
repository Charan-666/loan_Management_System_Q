namespace Kanini.LMP.Database.EntitiesDto.PaymentTransaction
{
    public class RazorpayOrderCreateDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Receipt { get; set; } = string.Empty;
        public int LoanAccountId { get; set; }
        public int EMIId { get; set; }
    }

    public class RazorpayOrderResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Receipt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long CreatedAt { get; set; }
    }

    public class RazorpayPaymentDto
    {
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;
        public int LoanAccountId { get; set; }
        public int EMIId { get; set; }
    }

    public class DisbursementDto
    {
        public int LoanAccountId { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string IfscCode { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public string Purpose { get; set; } = "loan_disbursement";
    }

    public class DisbursementResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public long CreatedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}
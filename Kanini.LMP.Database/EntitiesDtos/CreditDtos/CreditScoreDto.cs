using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Database.EntitiesDtos.CreditDtos
{
    public class CreditScoreDto
    {
        public int CustomerId { get; set; }
        public string PAN { get; set; } = null!;
        public CreditBureau Bureau { get; set; }
        public int Score { get; set; }
        public CreditScoreRange ScoreRange { get; set; }
        public DateTime FetchedAt { get; set; }
        public string? ReportSummary { get; set; }
        public List<CreditHistoryItem> CreditHistory { get; set; } = new();
    }

    public class CreditHistoryItem
    {
        public string AccountType { get; set; } = null!;
        public string Institution { get; set; } = null!;
        public decimal CreditLimit { get; set; }
        public decimal OutstandingAmount { get; set; }
        public CreditStatus Status { get; set; }
        public DateTime OpenedDate { get; set; }
        public int PaymentHistory { get; set; } // Percentage of on-time payments
    }

    public class CreditScoreRequest
    {
        public string PAN { get; set; } = null!;
    }
}
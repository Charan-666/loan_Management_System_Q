using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.ManagerEntitiesDtos
{
    public class RiskAssessmentReportDto
    {
        [Display(Name = "Report Period")]
        public string ReportPeriod { get; set; } = string.Empty;
        
        [Display(Name = "Total Applications")]
        public int TotalApplications { get; set; }
        
        [Display(Name = "High Risk Applications")]
        public int HighRiskApplications { get; set; }
        
        [Display(Name = "Medium Risk Applications")]
        public int MediumRiskApplications { get; set; }
        
        [Display(Name = "Low Risk Applications")]
        public int LowRiskApplications { get; set; }
        
        [Display(Name = "Average Credit Score")]
        public decimal AverageCreditScore { get; set; }
        
        [Display(Name = "Default Rate (%)")]
        public decimal DefaultRate { get; set; }
        
        [Display(Name = "Risk Distribution")]
        public List<RiskCategoryDto> RiskDistribution { get; set; } = new();
    }

    public class RiskCategoryDto
    {
        public string RiskLevel { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public decimal AverageAmount { get; set; }
    }

    public class ComplianceReportDto
    {
        [Display(Name = "Report Date")]
        public DateTime ReportDate { get; set; }
        
        [Display(Name = "Compliance Score (%)")]
        public decimal ComplianceScore { get; set; }
        
        [Display(Name = "KYC Completion Rate (%)")]
        public decimal KYCCompletionRate { get; set; }
        
        [Display(Name = "Document Verification Rate (%)")]
        public decimal DocumentVerificationRate { get; set; }
        
        [Display(Name = "Regulatory Violations")]
        public int RegulatoryViolations { get; set; }
        
        [Display(Name = "Audit Findings")]
        public List<AuditFindingDto> AuditFindings { get; set; } = new();
        
        [Display(Name = "Compliance Issues")]
        public List<ComplianceIssueDto> ComplianceIssues { get; set; } = new();
    }

    public class AuditFindingDto
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime IdentifiedDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ComplianceIssueDto
    {
        public string IssueType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ApplicationId { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Resolution { get; set; } = string.Empty;
    }

    public class LoanPerformanceReportDto
    {
        [Display(Name = "Report Period")]
        public string ReportPeriod { get; set; } = string.Empty;
        
        [Display(Name = "Total Active Loans")]
        public int TotalActiveLoans { get; set; }
        
        [Display(Name = "Total Loan Amount")]
        public decimal TotalLoanAmount { get; set; }
        
        [Display(Name = "On-Time Payment Rate (%)")]
        public decimal OnTimePaymentRate { get; set; }
        
        [Display(Name = "Overdue Loans")]
        public int OverdueLoans { get; set; }
        
        [Display(Name = "Default Loans")]
        public int DefaultLoans { get; set; }
        
        [Display(Name = "Portfolio Performance")]
        public List<PortfolioPerformanceDto> PortfolioPerformance { get; set; } = new();
    }

    public class PortfolioPerformanceDto
    {
        public string LoanType { get; set; } = string.Empty;
        public int ActiveLoans { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CollectionRate { get; set; }
        public int OverdueCount { get; set; }
    }
}
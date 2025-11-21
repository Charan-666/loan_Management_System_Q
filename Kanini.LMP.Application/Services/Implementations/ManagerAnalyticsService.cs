using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanCentricView;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.CustomerScape;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
using Kanini.LMP.Database.EntitiesDtos.ManagerEntitiesDtos;
using Kanini.LMP.Data.Data;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class ManagerAnalyticsService : IManagerAnalyticsService
    {
        private readonly LmpDbContext _context;

        public ManagerAnalyticsService(LmpDbContext context)
        {
            _context = context;
        }

        // Manager Dashboard Methods
        public async Task<OverallMetricsDto> GetOverallMetricsAsync()
        {
            var totalApplications = await _context.LoanApplicationBases.CountAsync();
            var approvedApplications = await _context.LoanApplicationBases
                .CountAsync(app => app.Status == ApplicationStatus.Approved);
            var pendingApplications = await _context.LoanOriginationWorkflows
                .CountAsync(w => w.StepStatus == StepStatus.InProgress);

            var successRate = totalApplications > 0 ? (decimal)approvedApplications / totalApplications * 100 : 0;
            var pendingRate = totalApplications > 0 ? (decimal)pendingApplications / totalApplications * 100 : 0;

            return new OverallMetricsDto
            {
                TotalApplications = totalApplications,
                AppSuccessRate = Math.Round(successRate, 1),
                CurrentAppRate = Math.Round(successRate, 1),
                AppPendingRate = Math.Round(pendingRate, 1)
            };
        }

        public async Task<IEnumerable<ApplicationStatusSummaryDto>> GetApplicationStatusSummaryAsync()
        {
            var statusCounts = await _context.LoanApplicationBases
                .GroupBy(app => app.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalCount = statusCounts.Sum(s => s.Count);

            return statusCounts.Select(s => new ApplicationStatusSummaryDto
            {
                StatusName = s.Status.ToString(),
                ApplicationCount = s.Count,
                Percentage = totalCount > 0 ? Math.Round((decimal)s.Count / totalCount * 100, 1) : 0
            });
        }

        public async Task<IEnumerable<ApplicationTrendDto>> GetApplicationTrendsAsync()
        {
            var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
            var applications = await _context.LoanApplicationBases
                .Where(app => app.SubmissionDate >= thirtyDaysAgo)
                .ToListAsync();

            var weeklyData = applications
                .GroupBy(app => 
                {
                    var daysDiff = app.SubmissionDate.DayNumber - thirtyDaysAgo.DayNumber;
                    return daysDiff / 7;
                })
                .Select(g => new ApplicationTrendDto
                {
                    PeriodLabel = $"Week {g.Key + 1}",
                    PeriodStartDate = thirtyDaysAgo.AddDays(g.Key * 7).ToDateTime(TimeOnly.MinValue),
                    MetricValue = g.Count()
                })
                .OrderBy(x => x.PeriodStartDate)
                .ToList();

            return weeklyData;
        }

        public async Task<IEnumerable<ApplicationTypePerformanceDto>> GetApplicationTypePerformanceAsync()
        {
            var thisMonth = DateTime.Now.Month;
            var lastMonth = DateTime.Now.AddMonths(-1).Month;
            var currentYear = DateTime.Now.Year;

            var loanTypes = await _context.LoanApplicationBases
                .GroupBy(app => app.LoanProductType)
                .Select(g => new {
                    LoanType = g.Key,
                    ThisMonthApproved = g.Count(app => app.Status == ApplicationStatus.Approved && 
                                              app.SubmissionDate.Month == thisMonth && 
                                              app.SubmissionDate.Year == currentYear),
                    ThisMonthTotal = g.Count(app => app.SubmissionDate.Month == thisMonth && 
                                           app.SubmissionDate.Year == currentYear),
                    LastMonthApproved = g.Count(app => app.Status == ApplicationStatus.Approved && 
                                              app.SubmissionDate.Month == lastMonth),
                    LastMonthTotal = g.Count(app => app.SubmissionDate.Month == lastMonth)
                })
                .ToListAsync();

            return loanTypes.Select(lt => new ApplicationTypePerformanceDto
            {
                LoanTypeName = lt.LoanType,
                ThisMonthValue = lt.ThisMonthTotal > 0 ? Math.Round((decimal)lt.ThisMonthApproved / lt.ThisMonthTotal * 100, 1) : 0,
                LastMonthValue = lt.LastMonthTotal > 0 ? Math.Round((decimal)lt.LastMonthApproved / lt.LastMonthTotal * 100, 1) : 0
            });
        }

        public async Task<NewApplicationsSummaryDto> GetNewApplicationsSummaryAsync()
        {
            var sevenDaysAgo = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
            
            var newApplications = await _context.LoanApplicationBases
                .Where(app => app.SubmissionDate >= sevenDaysAgo)
                .ToListAsync();

            var approvedCount = newApplications.Count(app => app.Status == ApplicationStatus.Approved);
            var pendingCount = newApplications.Count(app => app.Status == ApplicationStatus.Submitted);
            var totalNew = newApplications.Count;

            return new NewApplicationsSummaryDto
            {
                TotalNewApplications = totalNew,
                ApprovedCount = approvedCount,
                PendingCount = pendingCount,
                ApprovedPercentage = totalNew > 0 ? Math.Round((decimal)approvedCount / totalNew * 100, 1) : 0
            };
        }

        // Applied Loans Methods
        public async Task<IEnumerable<AppliedLoanListDto>> GetAppliedLoansAsync()
        {
            var applications = await _context.LoanApplicationBases
                .Join(_context.LoanApplicants, app => app.LoanApplicationBaseId, la => la.LoanApplicationBaseId, (app, la) => new { app, la })
                .Join(_context.Customers, x => x.la.CustomerId, c => c.CustomerId, (x, c) => new { x.app, x.la, c })
                .Join(_context.Users, x => x.c.UserId, u => u.UserId, (x, u) => new { x.app, x.la, x.c, u })
                .Select(x => new AppliedLoanListDto
                {
                    ApplicationId = x.app.LoanApplicationBaseId,
                    CustomerId = x.c.CustomerId,
                    CustomerFullName = x.u.FullName,
                    LoanProductType = x.app.LoanProductType,
                    ApplicationNumber = $"APP-{x.app.LoanApplicationBaseId}",
                    RequestedLoanAmount = 0, // Will be populated from LoanDetails if needed
                    SubmissionDate = x.app.SubmissionDate.ToDateTime(TimeOnly.MinValue),
                    Status = x.app.Status
                })
                .OrderByDescending(x => x.SubmissionDate)
                .ToListAsync();

            return applications;
        }

        public async Task<LoanApprovalDetailDto> GetLoanApprovalDetailAsync(int applicationId)
        {
            var application = await _context.LoanApplicationBases
                .Where(app => app.LoanApplicationBaseId == applicationId)
                .FirstOrDefaultAsync();

            if (application == null) return new LoanApprovalDetailDto();

            var workflow = await _context.LoanOriginationWorkflows
                .Where(w => w.LoanApplicationBaseId == applicationId)
                .OrderByDescending(w => w.CompletionDate)
                .FirstOrDefaultAsync();

            return new LoanApprovalDetailDto
            {
                LoanApplicationId = applicationId,
                ApplicationNumber = $"APP-{applicationId}",
                LoanProductType = application.LoanProductType,
                CurrentStatus = application.Status,
                CustomerFullName = "", // Will need to join with customer data
                CustomerId = "", // Will need to join with customer data
                CustomerOccupation = "",
                ProfileImageBase64 = "",
                RequestedLoanAmount = 0,
                RequestedTenureMonths = 0,
                EligibilityAmountCalculated = 0,
                EligibilityMessage = "",
                RevisedAmountOffered = 0,
                RevisedTenureMonthsOffered = 0,
                SignatureImageBase64 = "",
                IDProofImageBase64 = "",
                OriginationPipeline = new List<WorkflowStepDto>(),
                KYCEligibilityScore = 0,
                LoanDefaultScore = 0,
                OverallEligibilityScore = 0,
                ExplainerData = new ModelExplainerDataDto()
            };
        }

        // Customer Scape Methods - Placeholder implementations
        public async Task<CustomerScapeMasterDto> GetCustomerScapeAsync(int customerId)
        {
            return new CustomerScapeMasterDto();
        }

        public async Task<CustomerProfileDto> GetCustomerProfileAsync(int customerId)
        {
            return new CustomerProfileDto();
        }

        public async Task<IEnumerable<CustomerLoanSummaryDto>> GetCustomerLoanHistoryAsync(int customerId)
        {
            return new List<CustomerLoanSummaryDto>();
        }

        // Loan Analytics Methods
        public async Task<IEnumerable<LoanStatusDistributionDto>> GetLoanStatusDistributionAsync()
        {
            var statusDistribution = await _context.LoanApplicationBases
                .GroupBy(app => app.Status)
                .Select(g => new LoanStatusDistributionDto
                {
                    StatusName = g.Key.ToString(),
                    Percentage = 0 // Will be calculated after query
                })
                .ToListAsync();

            var total = statusDistribution.Count();
            var statusList = statusDistribution.ToList();
            for (int i = 0; i < statusList.Count; i++)
            {
                statusList[i].Percentage = total > 0 ? Math.Round(100.0m / total, 1) : 0;
            }

            return statusList;
        }

        public async Task<IEnumerable<LoanTypeDistributionDto>> GetLoanTypeDistributionAsync()
        {
            var typeDistribution = await _context.LoanApplicationBases
                .GroupBy(app => app.LoanProductType)
                .Select(g => new LoanTypeDistributionDto
                {
                    LoanTypeName = g.Key,
                    Percentage = 0 // Will be calculated after query
                })
                .ToListAsync();

            var total = typeDistribution.Count();
            var typeList = typeDistribution.ToList();
            for (int i = 0; i < typeList.Count; i++)
            {
                typeList[i].Percentage = total > 0 ? Math.Round(100.0m / total, 1) : 0;
            }

            return typeList;
        }

        public async Task<IEnumerable<MonthlyDistributionDto>> GetMonthlyDistributionAsync()
        {
            var sixMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6));
            var applications = await _context.LoanApplicationBases
                .Where(app => app.SubmissionDate >= sixMonthsAgo)
                .ToListAsync();

            var monthlyData = applications
                .GroupBy(app => new { app.SubmissionDate.Year, app.SubmissionDate.Month })
                .Select(g => new MonthlyDistributionDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Value = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            return monthlyData;
        }

        public async Task<IEnumerable<CountryDistributionDto>> GetCountryDistributionAsync()
        {
            // Since we don't have country data in current schema, return India-focused data
            var totalApplications = await _context.LoanApplicationBases.CountAsync();
            
            return new List<CountryDistributionDto>
            {
                new CountryDistributionDto
                {
                    Country = "India",
                    Percentage = 100.0m
                }
            };
        }

        public async Task<IEnumerable<DailyEngagementDto>> GetDailyEngagementAsync()
        {
            var sevenDaysAgo = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
            var applications = await _context.LoanApplicationBases
                .Where(app => app.SubmissionDate >= sevenDaysAgo)
                .ToListAsync();

            var dailyData = applications
                .GroupBy(app => app.SubmissionDate)
                .Select(g => new DailyEngagementDto
                {
                    DayOfMonth = g.Key.Day,
                    NumberOfLoans = g.Count(app => app.Status == ApplicationStatus.Approved || app.Status == ApplicationStatus.Rejected)
                })
                .OrderBy(x => x.DayOfMonth)
                .ToList();

            return dailyData;
        }

        // Loan Centric View Methods - Placeholder implementations
        public async Task<IEnumerable<LoanProductSummaryDto>> GetLoanProductSummaryAsync()
        {
            return new List<LoanProductSummaryDto>();
        }

        public async Task<LoanProductAnalysisDto> GetLoanProductAnalysisAsync(string loanProductType)
        {
            return new LoanProductAnalysisDto();
        }

        // Risk Assessment Reports
        public async Task<RiskAssessmentReportDto> GetRiskAssessmentReportAsync(DateTime fromDate, DateTime toDate)
        {
            var applications = await _context.LoanApplicationBases
                .Join(_context.LoanApplicants, app => app.LoanApplicationBaseId, la => la.LoanApplicationBaseId, (app, la) => new { app, la })
                .Join(_context.Customers, x => x.la.CustomerId, c => c.CustomerId, (x, c) => new { x.app, x.la, c })
                .Where(x => x.app.SubmissionDate >= DateOnly.FromDateTime(fromDate) && 
                           x.app.SubmissionDate <= DateOnly.FromDateTime(toDate))
                .ToListAsync();

            var totalApplications = applications.Count;
            var avgCreditScore = applications.Any() ? applications.Average(x => (decimal)x.c.CreditScore) : 0;
            
            // Risk categorization based on credit score
            var highRisk = applications.Count(x => x.c.CreditScore < 550);
            var mediumRisk = applications.Count(x => x.c.CreditScore >= 550 && x.c.CreditScore < 700);
            var lowRisk = applications.Count(x => x.c.CreditScore >= 700);
            
            // Calculate default rate (rejected applications as proxy)
            var defaultRate = totalApplications > 0 ? 
                (decimal)applications.Count(x => x.app.Status == ApplicationStatus.Rejected) / totalApplications * 100 : 0;

            var riskDistribution = new List<RiskCategoryDto>
            {
                new RiskCategoryDto { RiskLevel = "High Risk", Count = highRisk, Percentage = totalApplications > 0 ? (decimal)highRisk / totalApplications * 100 : 0 },
                new RiskCategoryDto { RiskLevel = "Medium Risk", Count = mediumRisk, Percentage = totalApplications > 0 ? (decimal)mediumRisk / totalApplications * 100 : 0 },
                new RiskCategoryDto { RiskLevel = "Low Risk", Count = lowRisk, Percentage = totalApplications > 0 ? (decimal)lowRisk / totalApplications * 100 : 0 }
            };

            return new RiskAssessmentReportDto
            {
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalApplications = totalApplications,
                HighRiskApplications = highRisk,
                MediumRiskApplications = mediumRisk,
                LowRiskApplications = lowRisk,
                AverageCreditScore = Math.Round(avgCreditScore, 0),
                DefaultRate = Math.Round(defaultRate, 2),
                RiskDistribution = riskDistribution
            };
        }

        // Compliance Reports
        public async Task<ComplianceReportDto> GetComplianceReportAsync()
        {
            var totalApplications = await _context.LoanApplicationBases.CountAsync();
            var documentsCount = await _context.ApplicationDocumentLinks.CountAsync();
            var verifiedDocuments = await _context.ApplicationDocumentLinks
                .CountAsync(d => d.Status == DocumentStatus.Verified);
            
            var kycCompletionRate = totalApplications > 0 ? (decimal)documentsCount / totalApplications * 100 : 0;
            var docVerificationRate = documentsCount > 0 ? (decimal)verifiedDocuments / documentsCount * 100 : 0;
            
            // Mock compliance score calculation
            var complianceScore = (kycCompletionRate + docVerificationRate) / 2;
            
            var auditFindings = new List<AuditFindingDto>
            {
                new AuditFindingDto
                {
                    Category = "Documentation",
                    Description = "Missing income verification documents",
                    Severity = "Medium",
                    IdentifiedDate = DateTime.UtcNow.AddDays(-15),
                    Status = "In Progress"
                }
            };

            return new ComplianceReportDto
            {
                ReportDate = DateTime.UtcNow,
                ComplianceScore = Math.Round(complianceScore, 1),
                KYCCompletionRate = Math.Round(kycCompletionRate, 1),
                DocumentVerificationRate = Math.Round(docVerificationRate, 1),
                RegulatoryViolations = 0,
                AuditFindings = auditFindings,
                ComplianceIssues = new List<ComplianceIssueDto>()
            };
        }

        // Loan Performance Reports
        public async Task<LoanPerformanceReportDto> GetLoanPerformanceReportAsync(DateTime fromDate, DateTime toDate)
        {
            var activeLoans = await _context.LoanAccounts
                .Where(la => la.CurrentPaymentStatus == LoanPaymentStatus.Active)
                .ToListAsync();

            var totalActiveLoans = activeLoans.Count;
            var totalLoanAmount = activeLoans.Sum(la => la.TotalLoanAmount);
            var overdueLoans = activeLoans.Count(la => la.DaysPastDue > 0);
            var defaultLoans = activeLoans.Count(la => la.CurrentPaymentStatus == LoanPaymentStatus.Defaulted);
            
            // Calculate on-time payment rate
            var onTimePaymentRate = totalActiveLoans > 0 ? 
                (decimal)(totalActiveLoans - overdueLoans) / totalActiveLoans * 100 : 0;

            // Portfolio performance by loan type
            var portfolioPerformance = await _context.LoanAccounts
                .Join(_context.LoanApplicationBases, la => la.LoanApplicationBaseId, app => app.LoanApplicationBaseId, (la, app) => new { la, app })
                .Where(x => x.la.CurrentPaymentStatus == LoanPaymentStatus.Active)
                .GroupBy(x => x.app.LoanProductType)
                .Select(g => new PortfolioPerformanceDto
                {
                    LoanType = g.Key,
                    ActiveLoans = g.Count(),
                    TotalAmount = g.Sum(x => x.la.TotalLoanAmount),
                    CollectionRate = g.Count() > 0 ? (decimal)g.Count(x => x.la.DaysPastDue == 0) / g.Count() * 100 : 0,
                    OverdueCount = g.Count(x => x.la.DaysPastDue > 0)
                })
                .ToListAsync();

            return new LoanPerformanceReportDto
            {
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalActiveLoans = totalActiveLoans,
                TotalLoanAmount = totalLoanAmount,
                OnTimePaymentRate = Math.Round(onTimePaymentRate, 1),
                OverdueLoans = overdueLoans,
                DefaultLoans = defaultLoans,
                PortfolioPerformance = portfolioPerformance
            };
        }
    }
}
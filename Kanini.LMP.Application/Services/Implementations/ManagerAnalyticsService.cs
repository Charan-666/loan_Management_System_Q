using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanCentricView;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.CustomerScape;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
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
                CurrentAppRate = Math.Round(successRate, 1), // Same as success rate for now
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

        // Applied Loans Methods - Placeholder implementations
        public async Task<IEnumerable<AppliedLoanListDto>> GetAppliedLoansAsync()
        {
            return new List<AppliedLoanListDto>();
        }

        public async Task<LoanApprovalDetailDto> GetLoanApprovalDetailAsync(int applicationId)
        {
            return new LoanApprovalDetailDto();
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

        // Loan Analytics Methods - Placeholder implementations
        public async Task<IEnumerable<LoanStatusDistributionDto>> GetLoanStatusDistributionAsync()
        {
            return new List<LoanStatusDistributionDto>();
        }

        public async Task<IEnumerable<LoanTypeDistributionDto>> GetLoanTypeDistributionAsync()
        {
            return new List<LoanTypeDistributionDto>();
        }

        public async Task<IEnumerable<MonthlyDistributionDto>> GetMonthlyDistributionAsync()
        {
            return new List<MonthlyDistributionDto>();
        }

        public async Task<IEnumerable<CountryDistributionDto>> GetCountryDistributionAsync()
        {
            return new List<CountryDistributionDto>();
        }

        public async Task<IEnumerable<DailyEngagementDto>> GetDailyEngagementAsync()
        {
            return new List<DailyEngagementDto>();
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
    }
}
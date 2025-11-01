using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanCentricView;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.CustomerScape;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IManagerAnalyticsService
    {
        // 1. Manager Dashboard
        Task<OverallMetricsDto> GetOverallMetricsAsync();
        Task<IEnumerable<ApplicationStatusSummaryDto>> GetApplicationStatusSummaryAsync();
        Task<IEnumerable<ApplicationTrendDto>> GetApplicationTrendsAsync();
        Task<IEnumerable<ApplicationTypePerformanceDto>> GetApplicationTypePerformanceAsync();
        Task<NewApplicationsSummaryDto> GetNewApplicationsSummaryAsync();

        // 2. Applied Loans
        Task<IEnumerable<AppliedLoanListDto>> GetAppliedLoansAsync();
        Task<LoanApprovalDetailDto> GetLoanApprovalDetailAsync(int applicationId);

        // 3. Customer Scape
        Task<CustomerScapeMasterDto> GetCustomerScapeAsync(int customerId);
        Task<CustomerProfileDto> GetCustomerProfileAsync(int customerId);
        Task<IEnumerable<CustomerLoanSummaryDto>> GetCustomerLoanHistoryAsync(int customerId);

        // 4. Loan Analytics
        Task<IEnumerable<LoanStatusDistributionDto>> GetLoanStatusDistributionAsync();
        Task<IEnumerable<LoanTypeDistributionDto>> GetLoanTypeDistributionAsync();
        Task<IEnumerable<MonthlyDistributionDto>> GetMonthlyDistributionAsync();
        Task<IEnumerable<CountryDistributionDto>> GetCountryDistributionAsync();
        Task<IEnumerable<DailyEngagementDto>> GetDailyEngagementAsync();

        // 5. Loan Centric View
        Task<IEnumerable<LoanProductSummaryDto>> GetLoanProductSummaryAsync();
        Task<LoanProductAnalysisDto> GetLoanProductAnalysisAsync(string loanProductType);
    }
}
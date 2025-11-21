using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos.ManagerEntitiesDtos;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class ReportsController : ControllerBase
    {
        private readonly IManagerAnalyticsService _analyticsService;

        public ReportsController(IManagerAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // Loan Performance Reports
        [HttpGet("loan-performance")]
        public async Task<ActionResult<LoanPerformanceReportDto>> GetLoanPerformanceReport(
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null)
        {
            var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
            var to = toDate ?? DateTime.UtcNow;
            
            var report = await _analyticsService.GetLoanPerformanceReportAsync(from, to);
            return Ok(report);
        }

        // Risk Assessment Reports
        [HttpGet("risk-assessment")]
        public async Task<ActionResult<RiskAssessmentReportDto>> GetRiskAssessmentReport(
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null)
        {
            var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
            var to = toDate ?? DateTime.UtcNow;
            
            var report = await _analyticsService.GetRiskAssessmentReportAsync(from, to);
            return Ok(report);
        }

        // Compliance Reports
        [HttpGet("compliance")]
        public async Task<ActionResult<ComplianceReportDto>> GetComplianceReport()
        {
            var report = await _analyticsService.GetComplianceReportAsync();
            return Ok(report);
        }

        // Customer Analytics Dashboard - Combined endpoint
        [HttpGet("customer-analytics")]
        public async Task<ActionResult> GetCustomerAnalyticsDashboard()
        {
            var loanStatusDistribution = await _analyticsService.GetLoanStatusDistributionAsync();
            var loanTypeDistribution = await _analyticsService.GetLoanTypeDistributionAsync();
            var monthlyDistribution = await _analyticsService.GetMonthlyDistributionAsync();
            var dailyEngagement = await _analyticsService.GetDailyEngagementAsync();

            return Ok(new
            {
                LoanStatusDistribution = loanStatusDistribution,
                LoanTypeDistribution = loanTypeDistribution,
                MonthlyTrends = monthlyDistribution,
                DailyEngagement = dailyEngagement,
                GeneratedAt = DateTime.UtcNow
            });
        }

        // Comprehensive Analytics Dashboard
        [HttpGet("analytics-dashboard")]
        public async Task<ActionResult> GetAnalyticsDashboard()
        {
            var overallMetrics = await _analyticsService.GetOverallMetricsAsync();
            var applicationStatus = await _analyticsService.GetApplicationStatusSummaryAsync();
            var applicationTrends = await _analyticsService.GetApplicationTrendsAsync();
            var typePerformance = await _analyticsService.GetApplicationTypePerformanceAsync();
            var newApplications = await _analyticsService.GetNewApplicationsSummaryAsync();

            return Ok(new
            {
                OverallMetrics = overallMetrics,
                ApplicationStatusSummary = applicationStatus,
                ApplicationTrends = applicationTrends,
                LoanTypePerformance = typePerformance,
                NewApplicationsSummary = newApplications,
                GeneratedAt = DateTime.UtcNow
            });
        }

        // Export Reports (Future enhancement)
        [HttpGet("export/{reportType}")]
        public async Task<ActionResult> ExportReport(string reportType, 
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string format = "json")
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
                var to = toDate ?? DateTime.UtcNow;

                object report = reportType.ToLower() switch
                {
                    "loan-performance" => await _analyticsService.GetLoanPerformanceReportAsync(from, to),
                    "risk-assessment" => await _analyticsService.GetRiskAssessmentReportAsync(from, to),
                    "compliance" => await _analyticsService.GetComplianceReportAsync(),
                    _ => throw new ArgumentException("Invalid report type")
                };

                // For now, return JSON. Future: Add CSV/PDF export
                return Ok(new
                {
                    ReportType = reportType,
                    Format = format,
                    GeneratedAt = DateTime.UtcNow,
                    Data = report
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating report: {ex.Message}");
            }
        }
    }
}
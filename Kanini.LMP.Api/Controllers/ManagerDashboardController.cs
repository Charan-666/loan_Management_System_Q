using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManagerDashboardController : ControllerBase
    {
        private readonly IManagerAnalyticsService _managerAnalyticsService;

        public ManagerDashboardController(IManagerAnalyticsService managerAnalyticsService)
        {
            _managerAnalyticsService = managerAnalyticsService;
        }

        // Removed - Frontend will compose dashboard from individual endpoints

        [HttpGet("overall-metrics")]
        public async Task<ActionResult<OverallMetricsDto>> GetOverallMetrics()
        {
            var metrics = await _managerAnalyticsService.GetOverallMetricsAsync();
            return Ok(metrics);
        }

        [HttpGet("application-status")]
        public async Task<ActionResult<IEnumerable<ApplicationStatusSummaryDto>>> GetApplicationStatusSummary()
        {
            var statusSummary = await _managerAnalyticsService.GetApplicationStatusSummaryAsync();
            return Ok(statusSummary);
        }

        [HttpGet("application-trends")]
        public async Task<ActionResult<IEnumerable<ApplicationTrendDto>>> GetApplicationTrends()
        {
            var trends = await _managerAnalyticsService.GetApplicationTrendsAsync();
            return Ok(trends);
        }

        [HttpGet("loan-type-performance")]
        public async Task<ActionResult<IEnumerable<ApplicationTypePerformanceDto>>> GetLoanTypePerformance()
        {
            var performance = await _managerAnalyticsService.GetApplicationTypePerformanceAsync();
            return Ok(performance);
        }

        [HttpGet("new-applications")]
        public async Task<ActionResult<NewApplicationsSummaryDto>> GetNewApplicationsSummary()
        {
            var summary = await _managerAnalyticsService.GetNewApplicationsSummaryAsync();
            return Ok(summary);
        }
    }
}
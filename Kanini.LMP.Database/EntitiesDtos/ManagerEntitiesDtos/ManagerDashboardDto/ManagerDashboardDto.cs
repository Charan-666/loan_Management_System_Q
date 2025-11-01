using System;
using System.Collections.Generic;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard
{
    public class ManagerDashboardDto
    {
        public OverallMetricsDto OverallMetrics { get; set; }
        public NewApplicationsSummaryDto NewApplicationsSummary { get; set; }
        public List<ApplicationStatusSummaryDto> ApplicationStatusBreakdown { get; set; }
        public List<ApplicationTrendDto> ApplicationTrends { get; set; }
        public List<ApplicationTypePerformanceDto> LoanTypePerformance { get; set; }
    }
}
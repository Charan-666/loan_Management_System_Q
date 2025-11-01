namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard
{
    public class OverallMetricsDto
    {
        public int TotalApplications { get; set; }
        public decimal AppSuccessRate { get; set; }  // (Total Approved / Total Submitted)
        public decimal CurrentAppRate { get; set; }
        public decimal AppPendingRate { get; set; } //review or pending
    }
}

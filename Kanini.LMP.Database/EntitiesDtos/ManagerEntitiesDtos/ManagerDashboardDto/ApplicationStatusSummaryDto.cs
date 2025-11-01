namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard
{
    public class ApplicationStatusSummaryDto
    {
        public string StatusName { get; set; } = string.Empty; //  "Closed", "Rejected"
        public int ApplicationCount { get; set; }
        public decimal Percentage { get; set; }
       
    }
}

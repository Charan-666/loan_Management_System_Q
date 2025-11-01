using System;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard
{
    public class ApplicationTrendDto
    {
        public string PeriodLabel { get; set; } = string.Empty; //  "Oct 24 - Oct 30"
        public DateTime PeriodStartDate { get; set; }
        
        public decimal MetricValue { get; set; }//  the total count 
    }
}


namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard
{
    public class ApplicationTypePerformanceDto
    {
        public string LoanTypeName { get; set; } = string.Empty; // take from LoanProductType in model
        public decimal ThisMonthValue { get; set; } //  Approval Rate for this month
        public decimal LastMonthValue { get; set; }  // Approval Rate for last month

    }
}


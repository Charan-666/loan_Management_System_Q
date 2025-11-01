using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans
{
    public class LoanRevisionInputDto
    {
        public int LoanApplicationId { get; set; }
        public decimal NewApprovedAmount { get; set; }
        public int NewTenureMonths { get; set; }
    }
  // Manager Service Logic:** The manager's API endpoint receives this DTO, updates the `RequestedAmount` and `TenureMonths` fields in the **`LoanDetails`** domain model, and potentially updates the status of the application in the **`LoanOriginationWorkflow`** model.
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.PaymentTransaction
{
    public class LoanAccountSummaryDTO
    {
        public int LoanAccountId { get; set; }
        public string LoanAccountNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalLoanAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int TotalEMIs { get; set; }
        public int PaidEMIs { get; set; }
        public int PendingEMIs => TotalEMIs - PaidEMIs;
        //public List<EMIDetailsDTO> EMISchedule { get; set; } = new();
    }
}

using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication
{
    public class HomeLoanApplicationSummaryDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public string LoanProductType { get; set; } = "HomeLoan";
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public string ApplicantName { get; set; } = null!;
    }
}

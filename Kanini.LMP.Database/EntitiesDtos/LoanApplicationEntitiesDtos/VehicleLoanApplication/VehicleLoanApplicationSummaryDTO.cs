using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication
{
    public class VehicleLoanApplicationSummaryDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string LoanProductType { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public decimal LoanAmount { get; set; }

    }
}

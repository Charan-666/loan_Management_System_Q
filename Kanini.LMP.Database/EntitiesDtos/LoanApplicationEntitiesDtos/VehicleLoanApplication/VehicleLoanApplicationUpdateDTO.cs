using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication
{
    public class VehicleLoanApplicationUpdateDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateOnly? ApprovedDate { get; set; }
    }
}

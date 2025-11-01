using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Enums
{
    public enum ManagerEnum
    {
        LoanApplied,
        Review,
        DocumentVerificationI,
        DocumentVerificationII,
        Decision,
        Disbursed
    }

    public enum StepStatus
    {
        Completed, 
        OnHold, 
        Failed, 
        InProgress
        
    }
}

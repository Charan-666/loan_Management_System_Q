using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans
{
    public class AppliedLoanListDto
    {
     
        public int ApplicationId { get; set; }

       
        public int CustomerId { get; set; }

      
        public string CustomerFullName { get; set; } = null!;

        
        public string LoanProductType { get; set; } = null!;

    
        public string ApplicationNumber { get; set; } = null!;

        
        public decimal RequestedLoanAmount { get; set; }

        public DateTime SubmissionDate { get; set; }

       
        public ApplicationStatus Status { get; set; }
    }
}

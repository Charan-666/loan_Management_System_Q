using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan
{
    public class EMIPlanDTO
    {
        public int EMIId { get; set; }

        public int LoanAppicationBaseId { get; set; }

        public int LoanAccountId { get; set; }

        [Display(Name = "Principal Amount (₹)")]
        public decimal PrincipleAmount { get; set; }

        [Display(Name = "Loan Term (Months)")]
        public int TermMonths { get; set; }

        [Display(Name = "Rate of Interest (%)")]
        public decimal RateOfInterest { get; set; }

        [Display(Name = "Monthly EMI (₹)")]
        public decimal MonthlyEMI { get; set; }

        [Display(Name = "Total Interest Paid (₹)")]
        public decimal TotalInerestPaid { get; set; }

        [Display(Name = "Total Repayment (₹)")]
        public decimal TotalRepaymentAmount { get; set; }

        public EMIPlanStatus Status { get; set; }

        [Display(Name = "Loan Fully Paid")]
        public bool IsCompleted { get; set; }
    }
}

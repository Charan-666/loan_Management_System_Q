using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan
{
    public class EMIPlanCreateDTO
    {
        [Required]
        public int LoanApplicationBaseId { get; set; }

        [Required]
        public int LoanAccountId { get; set; }

        [Required(ErrorMessage = "Principal Amount is required.")]
        [Range(1000, double.MaxValue, ErrorMessage = "Principal must be greater than ₹1,000.")]
        public decimal PrincipalAmount { get; set; }

        [Required(ErrorMessage = "Term (in months) is required.")]
        [Range(1, 360, ErrorMessage = "Loan term must be between 1 and 360 months.")]
        public int TermMonths { get; set; }

        [Required(ErrorMessage = "Interest Rate is required.")]
        [Range(1, 50, ErrorMessage = "Interest rate must be between 1% and 50%.")]
        public decimal RateOfInterest { get; set; }
    }
}

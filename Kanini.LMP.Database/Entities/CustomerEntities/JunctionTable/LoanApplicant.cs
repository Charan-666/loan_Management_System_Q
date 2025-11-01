using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable
{
    public class LoanApplicant
    {


        // Composite Key Part 1: FK to Loan Application
        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // Composite Key Part 2: FK to Customer
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        // --- Specific M:M Data ---
        [Required]
        public ApplicantRole ApplicantRole { get; set; } = ApplicantRole.Primary; 
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public LoanApplicationBase LoanApplicationBase { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}

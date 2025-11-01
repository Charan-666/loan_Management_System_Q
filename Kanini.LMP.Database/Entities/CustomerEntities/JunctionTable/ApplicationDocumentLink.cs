using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable
{
    /// <summary>
    /// M:M Join Table linking a specific LoanApplication to a DocumentUpload.
    /// This tracks which documents were submitted for a given application.
    /// </summary>
    public class ApplicationDocumentLink
    {
        // Composite Key Part 1 – FK to LoanApplication
        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // Composite Key Part 2 – FK to Document
        [ForeignKey(nameof(DocumentUpload))]
        public int DocumentId { get; set; }

        // Additional M:M Data
        [Required]
        public string DocumentRequirementType { get; set; } = "Unknown";  //e.g., 'This PDF is the Address Proof'

        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public LoanApplicationBase LoanApplicationBase { get; set; } = null!;
        public DocumentUpload DocumentUpload { get; set; } = null!;
    }

}

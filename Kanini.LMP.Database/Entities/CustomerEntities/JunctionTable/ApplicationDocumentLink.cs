using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
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
        public DocumentType DocumentRequirementType { get; set; } = DocumentType.Other;

        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
        
        // Document verification status
        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        
        public string? VerificationNotes { get; set; }
        
        public DateTime? VerifiedAt { get; set; }
        
        public int? VerifiedBy { get; set; } // Manager who verified

        // Navigation Properties
        public LoanApplicationBase LoanApplicationBase { get; set; } = null!;
        public DocumentUpload DocumentUpload { get; set; } = null!;
    }

}

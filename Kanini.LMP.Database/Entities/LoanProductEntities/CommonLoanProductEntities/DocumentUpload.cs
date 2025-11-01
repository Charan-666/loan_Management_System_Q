using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class DocumentUpload
    {
        [Key]
        public int DocumentId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // FK → User who uploaded the document
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Document metadata
        [Required]
        [MaxLength(255)]
        public string DocumentName { get; set; } = string.Empty;


        [MaxLength(100)]
        public string? DocumentType { get; set; }
        // Upload timestamp
        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        // Actual document binary data
        [Column(TypeName = "varbinary(max)")]
        public byte[]? DocumentData { get; set; }

        // Navigation: User who uploaded the document
        public virtual User? User { get; set; }


        /// <summary>
        /// Navigation property to the M:M Join Table (ApplicationDocumentLink).
        /// Used to see which applications this document is linked to.
        /// </summary>
        public ICollection<ApplicationDocumentLink> ApplicationLinks { get; set; } = new List<ApplicationDocumentLink>();
    }
}


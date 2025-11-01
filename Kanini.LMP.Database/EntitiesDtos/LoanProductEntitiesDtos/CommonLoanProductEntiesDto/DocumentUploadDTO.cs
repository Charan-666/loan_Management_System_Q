using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto
{
    public class DocumentUploadDTO
    {
        // Unique Document Identifier
        public int DocumentId { get; set; }

        // FK - Associated Loan Application
        [Required(ErrorMessage = "Loan Application ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        // FK - User who uploaded the document
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        // Document Metadata
        [Required(ErrorMessage = "Document name is required.")]
        [MaxLength(255, ErrorMessage = "Document name cannot exceed 255 characters.")]
        public string DocumentName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Document type cannot exceed 100 characters.")]
        public string? DocumentType { get; set; }

        // Upload Timestamp
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Base64-encoded string instead of byte[] for safe JSON transmission
        public string? DocumentDataBase64 { get; set; }

        // Optional: list of linked application IDs (for multi-link documents)
        public ICollection<int>? LinkedApplicationIds { get; set; }
    }
}

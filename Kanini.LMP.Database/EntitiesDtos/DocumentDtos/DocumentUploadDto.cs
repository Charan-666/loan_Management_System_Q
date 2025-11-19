using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Http;

namespace Kanini.LMP.Database.EntitiesDtos.DocumentDtos
{
    public class DocumentUploadDto
    {
        public int DocumentId { get; set; }
        public int LoanApplicationBaseId { get; set; }
        public int UserId { get; set; }
        public string DocumentName { get; set; } = null!;
        public string? DocumentType { get; set; }
        public DateTime UploadedAt { get; set; }
        public DocumentStatus Status { get; set; }
        public string? VerificationNotes { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public int? VerifiedBy { get; set; }
    }

    public class DocumentUploadRequest
    {
        public int LoanApplicationBaseId { get; set; }
        public string DocumentName { get; set; } = null!;
        public string DocumentType { get; set; } = null!;
        public DocumentType DocumentRequirementType { get; set; }
        public IFormFile File { get; set; } = null!;
    }

    public class DocumentVerificationRequest
    {
        public int LoanApplicationBaseId { get; set; }
        public int DocumentId { get; set; }
        public DocumentStatus Status { get; set; }
        public string? VerificationNotes { get; set; }
    }
}
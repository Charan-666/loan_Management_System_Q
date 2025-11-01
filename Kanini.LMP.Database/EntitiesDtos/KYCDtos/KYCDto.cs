using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDto.KYC
{
    public class KYCSubmissionDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int LoanApplicationId { get; set; }

        [Required]
        public string DocumentType { get; set; } = string.Empty; // "KYC_Aadhaar", "KYC_PAN", "KYC_Passport"

        [Required]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        public string DocumentImageBase64 { get; set; } = string.Empty;
    }

    public class KYCVerificationDto
    {
        public int DocumentId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentImageBase64 { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Verified, Rejected
    }

    public class KYCVerificationRequestDto
    {
        [Required]
        public int DocumentId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty; // Verified, Rejected

        [Required]
        public int ManagerId { get; set; }

        public string? Remarks { get; set; }
    }

    public class KYCStatusDto
    {
        public int CustomerId { get; set; }
        public bool IsKYCCompleted { get; set; }
        public List<KYCDocumentStatusDto> Documents { get; set; } = new();
        public decimal CompletionPercentage { get; set; }
    }

    public class KYCDocumentStatusDto
    {
        public string DocumentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? UploadedAt { get; set; }
        public string? DocumentName { get; set; }
    }
}
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.EntitiesDto.KYC;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class KYCService : IKYCService
    {
        private readonly ILMPRepository<DocumentUpload, int> _documentRepository;
        private readonly ILMPRepository<User, int> _userRepository;
        private readonly INotificationService _notificationService;

        // Required KYC document types
        private readonly List<string> _requiredKYCDocuments = new()
        {
            "KYC_Aadhaar",
            "KYC_PAN"
        };

        public KYCService(
            ILMPRepository<DocumentUpload, int> documentRepository,
            ILMPRepository<User, int> userRepository,
            INotificationService notificationService)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<bool> SubmitKYCDocumentAsync(KYCSubmissionDto kycDto)
        {
            try
            {
                // Convert base64 to byte array
                var documentData = Convert.FromBase64String(kycDto.DocumentImageBase64);

                var document = new DocumentUpload
                {
                    LoanApplicationBaseId = kycDto.LoanApplicationId,
                    UserId = kycDto.CustomerId,
                    DocumentName = kycDto.DocumentName,
                    DocumentType = kycDto.DocumentType, // "KYC_Aadhaar", "KYC_PAN", etc.
                    DocumentData = documentData,
                    UploadedAt = DateTime.UtcNow
                };

                await _documentRepository.AddAsync(document);

                // Notify manager for verification
                await _notificationService.NotifyManagerDocumentVerificationAsync(
                    3, // Manager ID (you may need to get this dynamically)
                    kycDto.LoanApplicationId,
                    "Customer");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<KYCStatusDto> GetCustomerKYCStatusAsync(int customerId)
        {
            var kycDocuments = await _documentRepository.GetAllAsync(
                d => d.UserId == customerId && d.DocumentType != null && d.DocumentType.StartsWith("KYC_"));

            var documentStatuses = _requiredKYCDocuments.Select(requiredDoc =>
            {
                var document = kycDocuments.FirstOrDefault(d => d.DocumentType == requiredDoc);
                return new KYCDocumentStatusDto
                {
                    DocumentType = requiredDoc.Replace("KYC_", ""),
                    Status = document != null ? "Submitted" : "Pending",
                    UploadedAt = document?.UploadedAt,
                    DocumentName = document?.DocumentName
                };
            }).ToList();

            var submittedCount = documentStatuses.Count(d => d.Status == "Submitted");
            var completionPercentage = (decimal)submittedCount / _requiredKYCDocuments.Count * 100;

            return new KYCStatusDto
            {
                CustomerId = customerId,
                IsKYCCompleted = submittedCount == _requiredKYCDocuments.Count,
                Documents = documentStatuses,
                CompletionPercentage = completionPercentage
            };
        }

        public async Task<IEnumerable<KYCVerificationDto>> GetPendingKYCDocumentsAsync()
        {
            var kycDocuments = await _documentRepository.GetAllAsync(
                d => d.DocumentType != null && d.DocumentType.StartsWith("KYC_"));

            var users = await _userRepository.GetAllAsync();

            return kycDocuments.Select(doc =>
            {
                var user = users.FirstOrDefault(u => u.UserId == doc.UserId);
                return new KYCVerificationDto
                {
                    DocumentId = doc.DocumentId,
                    CustomerId = doc.UserId,
                    CustomerName = user?.FullName ?? "Unknown",
                    DocumentType = doc.DocumentType?.Replace("KYC_", "") ?? "",
                    DocumentName = doc.DocumentName ?? "",
                    DocumentImageBase64 = doc.DocumentData != null ? Convert.ToBase64String(doc.DocumentData) : "",
                    UploadedAt = doc.UploadedAt,
                    Status = "Pending"
                };
            });
        }

        public async Task<KYCVerificationDto> GetKYCDocumentDetailsAsync(int documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null) throw new ArgumentException("Document not found");

            var user = await _userRepository.GetByIdAsync(document.UserId);

            return new KYCVerificationDto
            {
                DocumentId = document.DocumentId,
                CustomerId = document.UserId,
                CustomerName = user?.FullName ?? "Unknown",
                DocumentType = document.DocumentType?.Replace("KYC_", "") ?? "",
                DocumentName = document.DocumentName ?? "",
                DocumentImageBase64 = document.DocumentData != null ? Convert.ToBase64String(document.DocumentData) : "",
                UploadedAt = document.UploadedAt,
                Status = "Pending"
            };
        }

        public async Task<bool> VerifyKYCDocumentAsync(KYCVerificationRequestDto verificationDto)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(verificationDto.DocumentId);
                if (document == null) return false;

                // Update document name to indicate verification
                document.DocumentName = $"[VERIFIED] {document.DocumentName}";
                await _documentRepository.UpdateAsync(document);

                // Notify customer
                await _notificationService.CreateNotificationAsync(new Database.EntitiesDto.NotificationDTO
                {
                    UserId = document.UserId,
                    Title = "KYC Document Verified",
                    Message = $"Your {document.DocumentType?.Replace("KYC_", "")} document has been verified successfully."
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RejectKYCDocumentAsync(KYCVerificationRequestDto rejectionDto)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(rejectionDto.DocumentId);
                if (document == null) return false;

                // Update document name to indicate rejection
                document.DocumentName = $"[REJECTED] {document.DocumentName}";
                await _documentRepository.UpdateAsync(document);

                // Notify customer
                await _notificationService.CreateNotificationAsync(new Database.EntitiesDto.NotificationDTO
                {
                    UserId = document.UserId,
                    Title = "KYC Document Rejected",
                    Message = $"Your {document.DocumentType?.Replace("KYC_", "")} document has been rejected. Reason: {rejectionDto.Remarks ?? "Please resubmit with correct information."}"
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsKYCCompletedAsync(int customerId)
        {
            var kycDocuments = await _documentRepository.GetAllAsync(
                d => d.UserId == customerId && 
                     d.DocumentType != null && 
                     d.DocumentType.StartsWith("KYC_") &&
                     d.DocumentName != null &&
                     d.DocumentName.Contains("[VERIFIED]"));

            var verifiedDocumentTypes = kycDocuments
                .Select(d => d.DocumentType)
                .Distinct()
                .ToList();

            return _requiredKYCDocuments.All(required => 
                verifiedDocumentTypes.Contains(required));
        }

        public async Task<decimal> CalculateKYCScoreAsync(int customerId)
        {
            var kycDocuments = await _documentRepository.GetAllAsync(
                d => d.UserId == customerId && 
                     d.DocumentType != null && 
                     d.DocumentType.StartsWith("KYC_"));

            var verifiedCount = kycDocuments.Count(d => 
                d.DocumentName != null && d.DocumentName.Contains("[VERIFIED]"));

            var totalRequired = _requiredKYCDocuments.Count;
            
            return totalRequired > 0 ? (decimal)verifiedCount / totalRequired * 100 : 0;
        }
    }
}
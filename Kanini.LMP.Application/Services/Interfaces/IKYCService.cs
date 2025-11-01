using Kanini.LMP.Database.EntitiesDto.KYC;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IKYCService
    {
        // Customer KYC submission
        Task<bool> SubmitKYCDocumentAsync(KYCSubmissionDto kycDto);
        Task<KYCStatusDto> GetCustomerKYCStatusAsync(int customerId);
        
        // Manager KYC verification
        Task<IEnumerable<KYCVerificationDto>> GetPendingKYCDocumentsAsync();
        Task<KYCVerificationDto> GetKYCDocumentDetailsAsync(int documentId);
        Task<bool> VerifyKYCDocumentAsync(KYCVerificationRequestDto verificationDto);
        Task<bool> RejectKYCDocumentAsync(KYCVerificationRequestDto rejectionDto);
        
        // KYC validation
        Task<bool> IsKYCCompletedAsync(int customerId);
        Task<decimal> CalculateKYCScoreAsync(int customerId);
    }
}
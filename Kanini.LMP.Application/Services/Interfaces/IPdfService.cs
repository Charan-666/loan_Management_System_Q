namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateLoanApplicationPdfAsync(int applicationId);
        Task<byte[]> GeneratePaymentReceiptPdfAsync(int transactionId);
        Task<byte[]> GenerateEMISchedulePdfAsync(int loanAccountId);
    }
}
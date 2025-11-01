using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IRazorpayService
    {
        // EMI Payment Methods
        Task<RazorpayOrderResponseDto> CreateOrderAsync(RazorpayOrderCreateDto orderDto);
        Task<PaymentTransactionResponseDTO> ProcessPaymentAsync(RazorpayPaymentDto paymentDto);
        Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature);
        
        // Loan Disbursement Methods
        Task<DisbursementResponseDto> CreateDisbursementAsync(DisbursementDto disbursementDto);
        Task<DisbursementResponseDto> GetDisbursementStatusAsync(string disbursementId);
    }
}
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using EntityPaymentStatus = Kanini.LMP.Database.Entities.PaymentStatus;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentTransactionDTO> CreatePaymentAsync(PaymentTransactionCreateDTO dto);
        Task<IReadOnlyList<PaymentTransactionDTO>> GetPaymentsByLoanAccountAsync(int loanAccountId);
        Task<PaymentTransactionDTO> UpdatePaymentStatusAsync(int paymentId, EntityPaymentStatus status);
        Task<IReadOnlyList<EMIPlanDTO>> GetEMIPlansByLoanAccountAsync(int loanAccountId);
    }
}
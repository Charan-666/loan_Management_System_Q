using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using EntityPaymentStatus = Kanini.LMP.Database.Entities.PaymentStatus;
using EntityPaymentMethod = Kanini.LMP.Database.Entities.PaymentMethod;
using DtoPaymentStatus = Kanini.LMP.Database.EntitiesDto.PaymentTransaction.PaymentStatus;
using DtoPaymentMethod = Kanini.LMP.Database.EntitiesDto.PaymentTransaction.PaymentMethod;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly ILMPRepository<PaymentTransaction, int> _paymentRepository;
        private readonly ILMPRepository<EMIPlan, int> _emiRepository;

        public PaymentService(
            ILMPRepository<PaymentTransaction, int> paymentRepository,
            ILMPRepository<EMIPlan, int> emiRepository)
        {
            _paymentRepository = paymentRepository;
            _emiRepository = emiRepository;
        }

        public async Task<PaymentTransactionDTO> CreatePaymentAsync(PaymentTransactionCreateDTO dto)
        {
            var payment = new PaymentTransaction
            {
                EMIId = dto.EMIId,
                LoanAccountId = dto.LoanAccountId,
                Amount = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = (EntityPaymentMethod)dto.PaymentMethod,
                TransactionReference = dto.TransactionReference,
                Status = EntityPaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var created = await _paymentRepository.AddAsync(payment);
            return MapToDto(created);
        }

        public async Task<IReadOnlyList<PaymentTransactionDTO>> GetPaymentsByLoanAccountAsync(int loanAccountId)
        {
            var payments = await _paymentRepository.GetAllAsync(p => p.LoanAccountId == loanAccountId);
            return payments.Select(MapToDto).ToList();
        }

        public async Task<PaymentTransactionDTO> UpdatePaymentStatusAsync(int paymentId, EntityPaymentStatus status)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) throw new ArgumentException("Payment not found");

            payment.Status = status;
            payment.UpdatedAt = DateTime.UtcNow;

            var updated = await _paymentRepository.UpdateAsync(payment);
            return MapToDto(updated);
        }

        public async Task<IReadOnlyList<EMIPlanDTO>> GetEMIPlansByLoanAccountAsync(int loanAccountId)
        {
            var emiPlans = await _emiRepository.GetAllAsync(e => e.LoanAccountId == loanAccountId);
            return emiPlans.Select(MapEMIToDto).ToList();
        }

        private PaymentTransactionDTO MapToDto(PaymentTransaction payment)
        {
            return new PaymentTransactionDTO
            {
                TransactionId = payment.TransactionId,
                EMIId = payment.EMIId,
                LoanAccountId = payment.LoanAccountId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = (DtoPaymentMethod)payment.PaymentMethod,
                TransactionReference = payment.TransactionReference,
                Status = (DtoPaymentStatus)payment.Status,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt,
                IsActive = payment.IsActive
            };
        }

        private EMIPlanDTO MapEMIToDto(EMIPlan emi)
        {
            return new EMIPlanDTO
            {
                EMIId = emi.EMIId,
                LoanAppicationBaseId = emi.LoanApplicationBaseId,
                LoanAccountId = emi.LoanAccountId,
                PrincipleAmount = emi.PrincipleAmount,
                TermMonths = emi.TermMonths,
                RateOfInterest = emi.RateOfInterest,
                MonthlyEMI = emi.MonthlyEMI,
                TotalInerestPaid = emi.TotalInterestPaid,
                TotalRepaymentAmount = emi.TotalRepaymentAmount,
                Status = emi.Status,
                IsCompleted = emi.IsCompleted
            };
        }
    }
}
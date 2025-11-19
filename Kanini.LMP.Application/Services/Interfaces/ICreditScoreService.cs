using Kanini.LMP.Database.EntitiesDtos.CreditDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ICreditScoreService
    {
        Task<CreditScoreDto> GetCreditScoreAsync(int customerId);
        Task<CreditScoreDto> RefreshCreditScoreAsync(int customerId, string pan);
        Task<bool> UpdateCustomerCreditScoreAsync(int customerId, int creditScore);
    }
}
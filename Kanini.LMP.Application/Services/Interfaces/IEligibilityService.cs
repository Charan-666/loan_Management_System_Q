using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IEligibilityService
    {
        Task<EligibilityScoreDto> CalculateEligibilityAsync(int customerId, int loanProductId, string pan = null);
        Task<bool> IsEligibleForLoanAsync(int customerId, int loanProductId = 0);
        Task<List<int>> GetEligibleProductsAsync(int customerId);
    }
}
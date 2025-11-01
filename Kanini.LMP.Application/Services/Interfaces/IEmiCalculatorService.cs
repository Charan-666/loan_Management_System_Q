using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IEmiCalculatorService
    {
        Task<EMIPlanDTO> CalculateEmiAsync(decimal principalAmount, decimal interestRate, int termMonths);
        Task<EMIPlanDTO> CreateEmiPlanAsync(EMIPlanCreateDTO createDto);
        Task<EMIPlanDTO> GetEmiPlanByIdAsync(int emiId);
        Task<IEnumerable<EMIPlanDTO>> GetEmiPlansByLoanApplicationAsync(int loanApplicationId);
        Task<CustomerEMIDashboardDto?> GetCustomerEMIDashboardAsync(int customerId);
        Task<List<CustomerEMIDashboardDto>> GetAllCustomerEMIsAsync(int customerId);
    }
}
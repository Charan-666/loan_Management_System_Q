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
        
        // Advanced EMI Features
        Task<List<EMIScheduleDto>> GenerateEMIScheduleAsync(int emiId);
        Task<PrepaymentCalculationDto> CalculatePrepaymentAsync(int emiId, decimal prepaymentAmount);
        Task<decimal> CalculateLateFeeAsync(int emiId, DateTime currentDate);
        Task<EMIRestructureResultDto> CalculateEMIRestructureAsync(EMIRestructureDto restructureDto);
        Task<EMIPlanDTO> ApplyEMIRestructureAsync(EMIRestructureDto restructureDto);
        Task<object> GetCompleteEMIDetailsAsync(int emiId);
    }
}
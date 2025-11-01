using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ICustomerEMIService
    {
        Task<CustomerEMIDashboardDto?> GetCustomerEMIDashboardAsync(int customerId);
        Task<List<CustomerEMIDashboardDto>> GetAllCustomerEMIsAsync(int customerId);
    }
}
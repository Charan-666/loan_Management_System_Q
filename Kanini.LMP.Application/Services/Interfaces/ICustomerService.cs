using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ICustomerService : ILMPService<CustomerDto, int>
    {
        Task<CustomerDto?> GetByUserIdAsync(int userId);
    }
}
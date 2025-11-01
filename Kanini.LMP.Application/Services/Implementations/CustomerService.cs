using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ILMPRepository<Customer, int> _customerRepository;

        public CustomerService(ILMPRepository<Customer, int> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto> Add(CustomerDto entity)
        {
            var customer = new Customer
            {
                UserId = entity.UserId,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                PhoneNumber = entity.PhoneNumber,
                Occupation = entity.Occupation,
                AnnualIncome = entity.AnnualIncome,
                CreditScore = entity.CreditScore,
                HomeOwnershipStatus = entity.HomeOwnershipStatus,
                ProfileImage = new byte[] { 0x00 }, // Default empty image
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _customerRepository.AddAsync(customer);
            return MapToDto(created);
        }

        public async Task Delete(int id)
        {
            await _customerRepository.DeleteAsync(id);
        }

        public async Task<IReadOnlyList<CustomerDto>> GetAll()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(MapToDto).ToList();
        }

        public async Task<CustomerDto?> GetById(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer != null ? MapToDto(customer) : null;
        }

        public async Task<CustomerDto> Update(CustomerDto entity)
        {
            var customer = new Customer
            {
                CustomerId = entity.CustomerId,
                UserId = entity.UserId,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                PhoneNumber = entity.PhoneNumber,
                Occupation = entity.Occupation,
                AnnualIncome = entity.AnnualIncome,
                CreditScore = entity.CreditScore,
                HomeOwnershipStatus = entity.HomeOwnershipStatus,
                UpdatedAt = DateTime.UtcNow
            };

            var updated = await _customerRepository.UpdateAsync(customer);
            return MapToDto(updated);
        }

        public async Task<CustomerDto?> GetByUserIdAsync(int userId)
        {
            var customer = await _customerRepository.GetAsync(c => c.UserId == userId);
            return customer != null ? MapToDto(customer) : null;
        }

        private CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                UserId = customer.UserId,
                DateOfBirth = customer.DateOfBirth,
                Gender = customer.Gender,
                PhoneNumber = customer.PhoneNumber,
                Occupation = customer.Occupation,
                AnnualIncome = customer.AnnualIncome,
                CreditScore = customer.CreditScore,
                HomeOwnershipStatus = customer.HomeOwnershipStatus,
                UpdatedAt = customer.UpdatedAt,
                Age = customer.Age
            };
        }
    }
}
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAllCustomers()
        {
            var customers = await _customerService.GetAll();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _customerService.GetById(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerByUserId(int userId)
        {
            var customer = await _customerService.GetByUserIdAsync(userId);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerDto customerDto)
        {
            var created = await _customerService.Add(customerDto);
            return CreatedAtAction(nameof(GetCustomer), new { id = created.CustomerId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, CustomerDto customerDto)
        {
            if (id != customerDto.CustomerId) return BadRequest();
            var updated = await _customerService.Update(customerDto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _customerService.Delete(id);
            return NoContent();
        }
    }
}
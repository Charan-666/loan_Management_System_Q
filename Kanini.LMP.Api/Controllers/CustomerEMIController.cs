using Kanini.LMP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CustomerEMIController : ControllerBase
    {
        private readonly ICustomerEMIService _customerEMIService;

        public CustomerEMIController(ICustomerEMIService customerEMIService)
        {
            _customerEMIService = customerEMIService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetEMIDashboard()
        {
            try
            {
                var customerIdClaim = User.FindFirst("CustomerId")?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                {
                    return Unauthorized("Customer ID not found in token");
                }

                var dashboard = await _customerEMIService.GetCustomerEMIDashboardAsync(customerId);
                if (dashboard == null)
                {
                    return NotFound("No active EMI found for customer");
                }

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEMIs()
        {
            try
            {
                var customerIdClaim = User.FindFirst("CustomerId")?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                {
                    return Unauthorized("Customer ID not found in token");
                }

                var emis = await _customerEMIService.GetAllCustomerEMIsAsync(customerId);
                return Ok(emis);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
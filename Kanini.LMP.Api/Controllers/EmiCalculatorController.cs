using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmiCalculatorController : ControllerBase
    {
        private readonly IEmiCalculatorService _emiCalculatorService;

        public EmiCalculatorController(IEmiCalculatorService emiCalculatorService)
        {
            _emiCalculatorService = emiCalculatorService;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<EMIPlanDTO>> CalculateEmi([FromBody] CalculateEmiRequest request)
        {
            var result = await _emiCalculatorService.CalculateEmiAsync(request.PrincipalAmount, request.InterestRate, request.TermMonths);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult<EMIPlanDTO>> CreateEmiPlan([FromBody] EMIPlanCreateDTO createDto)
        {
            var result = await _emiCalculatorService.CreateEmiPlanAsync(createDto);
            return Ok(result);
        }

        [HttpGet("{emiId}")]
        public async Task<ActionResult<EMIPlanDTO>> GetEmiPlan(int emiId)
        {
            var result = await _emiCalculatorService.GetEmiPlanByIdAsync(emiId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("loan/{loanApplicationId}")]
        public async Task<ActionResult<IEnumerable<EMIPlanDTO>>> GetEmiPlansByLoan(int loanApplicationId)
        {
            var result = await _emiCalculatorService.GetEmiPlansByLoanApplicationAsync(loanApplicationId);
            return Ok(result);
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetEMIDashboard()
        {
            try
            {
                var customerIdClaim = User.FindFirst("CustomerId")?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                {
                    return Unauthorized("Customer ID not found in token");
                }

                var dashboard = await _emiCalculatorService.GetCustomerEMIDashboardAsync(customerId);
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllEMIs()
        {
            try
            {
                var customerIdClaim = User.FindFirst("CustomerId")?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                {
                    return Unauthorized("Customer ID not found in token");
                }

                var emis = await _emiCalculatorService.GetAllCustomerEMIsAsync(customerId);
                return Ok(emis);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{emiId}/schedule")]
        public async Task<IActionResult> GetEMISchedule(int emiId)
        {
            var schedule = await _emiCalculatorService.GenerateEMIScheduleAsync(emiId);
            return Ok(schedule);
        }

        [HttpPost("{emiId}/prepayment")]
        public async Task<IActionResult> CalculatePrepayment(int emiId, [FromBody] PrepaymentRequest request)
        {
            var calculation = await _emiCalculatorService.CalculatePrepaymentAsync(emiId, request.PrepaymentAmount);
            return Ok(calculation);
        }

        [HttpGet("{emiId}/latefee")]
        public async Task<IActionResult> CalculateLateFee(int emiId)
        {
            var lateFee = await _emiCalculatorService.CalculateLateFeeAsync(emiId, DateTime.UtcNow);
            return Ok(new { LateFee = lateFee });
        }

        [HttpPost("restructure/calculate")]
        public async Task<IActionResult> CalculateRestructure([FromBody] EMIRestructureDto restructureDto)
        {
            var result = await _emiCalculatorService.CalculateEMIRestructureAsync(restructureDto);
            return Ok(result);
        }

        [HttpPost("restructure/apply")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApplyRestructure([FromBody] EMIRestructureDto restructureDto)
        {
            var result = await _emiCalculatorService.ApplyEMIRestructureAsync(restructureDto);
            return Ok(result);
        }

        [HttpGet("{emiId}/complete-details")]
        public async Task<IActionResult> GetCompleteEMIDetails(int emiId)
        {
            var result = await _emiCalculatorService.GetCompleteEMIDetailsAsync(emiId);
            return result != null ? Ok(result) : NotFound();
        }
    }

    public class CalculateEmiRequest
    {
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TermMonths { get; set; }
    }

    public class PrepaymentRequest
    {
        public decimal PrepaymentAmount { get; set; }
    }
}
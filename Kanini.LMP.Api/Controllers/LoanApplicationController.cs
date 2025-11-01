using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoanApplicationController : ControllerBase
    {
        private readonly ILoanApplicationService _loanApplicationService;

        public LoanApplicationController(ILoanApplicationService loanApplicationService)
        {
            _loanApplicationService = loanApplicationService;
        }

        [HttpGet("personal")]
        public async Task<ActionResult<IReadOnlyList<PersonalLoanApplicationDTO>>> GetAllPersonalLoans()
        {
            var loans = await _loanApplicationService.GetAllPersonalLoansAsync();
            return Ok(loans);
        }

        [HttpGet("personal/{id}")]
        public async Task<ActionResult<PersonalLoanApplicationDTO>> GetPersonalLoan(int id)
        {
            var loan = await _loanApplicationService.GetPersonalLoanByIdAsync(id);
            if (loan == null) return NotFound();
            return Ok(loan);
        }

        [HttpGet("personal/status/{status}")]
        public async Task<ActionResult<IReadOnlyList<PersonalLoanApplicationDTO>>> GetLoansByStatus(ApplicationStatus status)
        {
            var loans = await _loanApplicationService.GetLoansByStatusAsync(status);
            return Ok(loans);
        }

        [HttpPost("personal/{customerId}/submit")]
        public async Task<ActionResult> SubmitCompletePersonalLoan(int customerId, PersonalLoanApplicationCreateDTO dto)
        {
            try
            {
                // Create application with all details
                var created = await _loanApplicationService.CreatePersonalLoanAsync(dto, customerId);
                
                // Update status to Submitted
                var submitted = await _loanApplicationService.UpdateLoanStatusAsync(created.LoanApplicationBaseId, ApplicationStatus.Submitted);
                
                return Ok(new 
                { 
                    ApplicationId = submitted.LoanApplicationBaseId,
                    Status = submitted.Status,
                    Message = "Loan application submitted successfully. You will be notified about the status.",
                    NextSteps = new[] 
                    {
                        "Upload required documents using /api/Document/upload endpoint",
                        "Wait for verification and approval",
                        "Check application status regularly"
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("personal/{id}/status")]
        public async Task<ActionResult<PersonalLoanApplicationDTO>> UpdateLoanStatus(int id, [FromBody] ApplicationStatus status)
        {
            var updated = await _loanApplicationService.UpdateLoanStatusAsync(id, status);
            return Ok(updated);
        }

        // Home Loan Endpoints
        [HttpPost("home/{customerId}")]
        public async Task<ActionResult<HomeLoanApplicationDTO>> CreateHomeLoan(int customerId, HomeLoanApplicationCreateDTO dto)
        {
            try
            {
                var created = await _loanApplicationService.CreateHomeLoanAsync(dto, customerId);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Vehicle Loan Endpoints
        [HttpPost("vehicle/{customerId}")]
        public async Task<ActionResult<VehicleLoanApplicationDTO>> CreateVehicleLoan(int customerId, VehicleLoanApplicationCreateDTO dto)
        {
            try
            {
                var created = await _loanApplicationService.CreateVehicleLoanAsync(dto, customerId);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
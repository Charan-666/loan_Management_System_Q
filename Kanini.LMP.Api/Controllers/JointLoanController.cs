using Kanini.LMP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JointLoanController : ControllerBase
    {
        private readonly ILoanApplicationService _loanApplicationService;

        public JointLoanController(ILoanApplicationService loanApplicationService)
        {
            _loanApplicationService = loanApplicationService;
        }

        [HttpGet("{loanApplicationId}/applicants")]
        public async Task<ActionResult> GetLoanApplicants(int loanApplicationId)
        {
            var applicants = await _loanApplicationService.GetApplicantsByLoanAsync(loanApplicationId);
            return Ok(new { LoanApplicationId = loanApplicationId, ApplicantIds = applicants });
        }

        [HttpGet("{loanApplicationId}/documents")]
        public async Task<ActionResult> GetLoanDocuments(int loanApplicationId)
        {
            var documents = await _loanApplicationService.GetDocumentsByLoanAsync(loanApplicationId);
            return Ok(new { LoanApplicationId = loanApplicationId, DocumentIds = documents });
        }
    }
}
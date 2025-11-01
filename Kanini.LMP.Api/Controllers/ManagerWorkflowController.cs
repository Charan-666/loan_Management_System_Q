using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboardDto.Manager.NewFolderBasicDto;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManagerWorkflowController : ControllerBase
    {
        private readonly IManagerWorkflowService _managerWorkflowService;

        public ManagerWorkflowController(IManagerWorkflowService managerWorkflowService)
        {
            _managerWorkflowService = managerWorkflowService;
        }

        [HttpGet("pending-applications")]
        public async Task<ActionResult<IEnumerable<AppliedLoanListDto>>> GetPendingApplications()
        {
            var applications = await _managerWorkflowService.GetPendingApplicationsAsync();
            return Ok(applications);
        }

        [HttpGet("application/{applicationId}")]
        public async Task<ActionResult<LoanApprovalDetailDto>> GetApplicationDetails(int applicationId)
        {
            var details = await _managerWorkflowService.GetApplicationDetailsAsync(applicationId);
            return Ok(details);
        }

        [HttpPost("start-workflow/{applicationId}/{managerId}")]
        public async Task<ActionResult<LoanOriginationWorkflowDTO>> StartWorkflow(int applicationId, int managerId)
        {
            var workflow = await _managerWorkflowService.StartWorkflowAsync(applicationId, managerId);
            return Ok(workflow);
        }

        [HttpPut("update-step/{applicationId}")]
        public async Task<ActionResult<LoanOriginationWorkflowDTO>> UpdateWorkflowStep(
            int applicationId, 
            [FromBody] UpdateStepRequest request)
        {
            var workflow = await _managerWorkflowService.UpdateWorkflowStepAsync(
                applicationId, request.StepName, request.Status, request.Notes, request.ManagerId);
            return Ok(workflow);
        }

        [HttpGet("workflow-status/{applicationId}")]
        public async Task<ActionResult<IEnumerable<WorkflowStepDto>>> GetWorkflowStatus(int applicationId)
        {
            var status = await _managerWorkflowService.GetWorkflowStatusAsync(applicationId);
            return Ok(status);
        }

        [HttpPost("verify-documents/{applicationId}/{managerId}")]
        public async Task<ActionResult> VerifyDocuments(int applicationId, int managerId, [FromBody] string verificationNotes)
        {
            var result = await _managerWorkflowService.VerifyDocumentsAsync(applicationId, managerId, verificationNotes);
            return Ok(new { Success = result, Message = "Documents verified successfully" });
        }

        [HttpPost("approve/{applicationId}/{managerId}")]
        public async Task<ActionResult> ApproveApplication(int applicationId, int managerId, [FromBody] string approvalNotes)
        {
            var result = await _managerWorkflowService.ApproveApplicationAsync(applicationId, managerId, approvalNotes);
            return Ok(new { Success = result, Message = "Application approved successfully" });
        }

        [HttpPost("reject/{applicationId}/{managerId}")]
        public async Task<ActionResult> RejectApplication(int applicationId, int managerId, [FromBody] string rejectionReason)
        {
            var result = await _managerWorkflowService.RejectApplicationAsync(applicationId, managerId, rejectionReason);
            return Ok(new { Success = result, Message = "Application rejected" });
        }

        [HttpPost("revise/{applicationId}/{managerId}")]
        public async Task<ActionResult> ReviseApplication(int applicationId, int managerId, [FromBody] LoanRevisionInputDto revisionData)
        {
            var result = await _managerWorkflowService.ReviseApplicationAsync(applicationId, revisionData, managerId);
            return Ok(new { Success = result, Message = "Application revised successfully" });
        }

        [HttpPost("disburse/{applicationId}/{managerId}")]
        public async Task<ActionResult> DisburseApplication(int applicationId, int managerId, [FromBody] decimal disbursedAmount)
        {
            var result = await _managerWorkflowService.DisburseApplicationAsync(applicationId, managerId, disbursedAmount);
            return Ok(new { Success = result, Message = "Loan disbursed successfully" });
        }
    }

    public class UpdateStepRequest
    {
        public ManagerEnum StepName { get; set; }
        public StepStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int ManagerId { get; set; }
    }
}
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboardDto.Manager.NewFolderBasicDto;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IManagerWorkflowService
    {
        // Application Management
        Task<IEnumerable<AppliedLoanListDto>> GetPendingApplicationsAsync();
        Task<LoanApprovalDetailDto> GetApplicationDetailsAsync(int applicationId);
        
        // Workflow Management
        Task<LoanOriginationWorkflowDTO> StartWorkflowAsync(int applicationId, int managerId);
        Task<LoanOriginationWorkflowDTO> UpdateWorkflowStepAsync(int applicationId, ManagerEnum stepName, StepStatus status, string notes, int managerId);
        Task<IEnumerable<WorkflowStepDto>> GetWorkflowStatusAsync(int applicationId);
        
        // Document Verification
        Task<bool> VerifyDocumentsAsync(int applicationId, int managerId, string verificationNotes);
        
        // Decision Making
        Task<bool> ApproveApplicationAsync(int applicationId, int managerId, string approvalNotes);
        Task<bool> RejectApplicationAsync(int applicationId, int managerId, string rejectionReason);
        Task<bool> ReviseApplicationAsync(int applicationId, LoanRevisionInputDto revisionData, int managerId);
        
        // Disbursement
        Task<bool> DisburseApplicationAsync(int applicationId, int managerId, decimal disbursedAmount);
    }
}
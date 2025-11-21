using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboardDto.Manager.NewFolderBasicDto;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class ManagerWorkflowService : IManagerWorkflowService
    {
        private readonly ILMPRepository<LoanOriginationWorkflow, int> _workflowRepository;
        private readonly ILMPRepository<LoanApplicationBase, int> _applicationRepository;
        private readonly ILMPRepository<Customer, int> _customerRepository;
        private readonly ILMPRepository<User, int> _userRepository;
        private readonly ILMPRepository<LoanDetails, int> _loanDetailsRepository;
        private readonly ILMPRepository<LoanAccount, int> _loanAccountRepository;
        private readonly INotificationService _notificationService;
        private readonly IRazorpayService _razorpayService;

        public ManagerWorkflowService(
            ILMPRepository<LoanOriginationWorkflow, int> workflowRepository,
            ILMPRepository<LoanApplicationBase, int> applicationRepository,
            ILMPRepository<Customer, int> customerRepository,
            ILMPRepository<User, int> userRepository,
            ILMPRepository<LoanDetails, int> loanDetailsRepository,
            ILMPRepository<LoanAccount, int> loanAccountRepository,
            INotificationService notificationService,
            IRazorpayService razorpayService)
        {
            _workflowRepository = workflowRepository;
            _applicationRepository = applicationRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _loanDetailsRepository = loanDetailsRepository;
            _loanAccountRepository = loanAccountRepository;
            _notificationService = notificationService;
            _razorpayService = razorpayService;
        }

        public async Task<IEnumerable<AppliedLoanListDto>> GetPendingApplicationsAsync()
        {
            var applications = await _applicationRepository.GetAllAsync();
            var customers = await _customerRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();

            return applications
                .Where(app => app.Status == ApplicationStatus.Submitted || app.Status == ApplicationStatus.Pending)
                .Select(app =>
                {
                    var customer = customers.FirstOrDefault(c => c.CustomerId == app.Applicants.FirstOrDefault()?.CustomerId);
                    var user = users.FirstOrDefault(u => u.UserId == customer?.UserId);
                    
                    return new AppliedLoanListDto
                    {
                        ApplicationId = app.LoanApplicationBaseId,
                        CustomerId = customer?.CustomerId ?? 0,
                        CustomerFullName = user?.FullName ?? "Unknown",
                        LoanProductType = app.LoanProductType,
                        ApplicationNumber = $"LMP{app.LoanApplicationBaseId:D6}",
                        RequestedLoanAmount = app.LoanDetails?.RequestedAmount ?? 0,
                        SubmissionDate = app.SubmissionDate.ToDateTime(TimeOnly.MinValue),
                        Status = app.Status
                    };
                });
        }

        public async Task<LoanApprovalDetailDto> GetApplicationDetailsAsync(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            var workflows = await _workflowRepository.GetAllAsync();
            var customers = await _customerRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();
            var loanDetails = await _loanDetailsRepository.GetAllAsync();

            var customer = customers.FirstOrDefault(c => c.CustomerId == application.Applicants.FirstOrDefault()?.CustomerId);
            var user = users.FirstOrDefault(u => u.UserId == customer?.UserId);
            var loanDetail = loanDetails.FirstOrDefault(ld => ld.LoanApplicationBaseId == applicationId);
            var appWorkflows = workflows.Where(w => w.LoanApplicationBaseId == applicationId);

            return new LoanApprovalDetailDto
            {
                LoanApplicationId = applicationId,
                ApplicationNumber = $"LMP{applicationId:D6}",
                LoanProductType = application.LoanProductType,
                CurrentStatus = application.Status,
                CustomerFullName = user?.FullName ?? "Unknown",
                CustomerId = customer?.CustomerId.ToString() ?? "0",
                CustomerOccupation = customer?.Occupation ?? "Unknown",
                ProfileImageBase64 = Convert.ToBase64String(customer?.ProfileImage ?? new byte[0]),
                RequestedLoanAmount = loanDetail?.RequestedAmount ?? 0,
                RequestedTenureMonths = loanDetail?.TenureMonths ?? 0,
                EligibilityAmountCalculated = loanDetail?.RequestedAmount ?? 0,
                EligibilityMessage = "Eligible based on credit score and income",
                RevisedAmountOffered = loanDetail?.RequestedAmount ?? 0,
                RevisedTenureMonthsOffered = loanDetail?.TenureMonths ?? 0,
                SignatureImageBase64 = "",
                IDProofImageBase64 = "",
                OriginationPipeline = appWorkflows.Select(w => new WorkflowStepDto
                {
                    StepName = w.StepName.ToString(),
                    CompletionDate = w.CompletionDate,
                    IsCompleted = w.StepStatus == StepStatus.Completed,
                    StatusIndicator = w.StepStatus.ToString()
                }).ToList(),
                KYCEligibilityScore = customer?.CreditScore ?? 0,
                LoanDefaultScore = 85.0m,
                OverallEligibilityScore = customer?.CreditScore ?? 0
            };
        }

        public async Task<LoanOriginationWorkflowDTO> StartWorkflowAsync(int applicationId, int managerId)
        {
            var workflow = new LoanOriginationWorkflow
            {
                LoanApplicationBaseId = applicationId,
                StepName = ManagerEnum.Review,
                StepStatus = StepStatus.InProgress,
                ManagerId = managerId,
                ManagerNotes = "Workflow started - Initial review in progress"
            };

            var created = await _workflowRepository.AddAsync(workflow);
            
            // Update application status
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            application.Status = ApplicationStatus.Pending;
            await _applicationRepository.UpdateAsync(application);

            return MapToWorkflowDto(created);
        }

        public async Task<LoanOriginationWorkflowDTO> UpdateWorkflowStepAsync(int applicationId, ManagerEnum stepName, StepStatus status, string notes, int managerId)
        {
            var workflows = await _workflowRepository.GetAllAsync();
            var workflow = workflows.FirstOrDefault(w => w.LoanApplicationBaseId == applicationId && w.StepName == stepName);

            if (workflow == null)
            {
                workflow = new LoanOriginationWorkflow
                {
                    LoanApplicationBaseId = applicationId,
                    StepName = stepName,
                    StepStatus = status,
                    ManagerId = managerId,
                    ManagerNotes = notes,
                    CompletionDate = status == StepStatus.Completed ? DateTime.UtcNow : null
                };
                workflow = await _workflowRepository.AddAsync(workflow);
            }
            else
            {
                workflow.StepStatus = status;
                workflow.ManagerNotes = notes;
                workflow.ManagerId = managerId;
                workflow.CompletionDate = status == StepStatus.Completed ? DateTime.UtcNow : workflow.CompletionDate;
                workflow = await _workflowRepository.UpdateAsync(workflow);
            }

            return MapToWorkflowDto(workflow);
        }

        public async Task<IEnumerable<WorkflowStepDto>> GetWorkflowStatusAsync(int applicationId)
        {
            var workflows = await _workflowRepository.GetAllAsync();
            var appWorkflows = workflows.Where(w => w.LoanApplicationBaseId == applicationId);

            return appWorkflows.Select(w => new WorkflowStepDto
            {
                StepName = w.StepName.ToString(),
                CompletionDate = w.CompletionDate,
                IsCompleted = w.StepStatus == StepStatus.Completed,
                StatusIndicator = w.StepStatus.ToString()
            });
        }

        public async Task<bool> VerifyDocumentsAsync(int applicationId, int managerId, string verificationNotes)
        {
            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.DocumentVerificationI, StepStatus.Completed, verificationNotes, managerId);
            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.DocumentVerificationII, StepStatus.InProgress, "Starting detailed verification", managerId);
            return true;
        }

        public async Task<bool> ApproveApplicationAsync(int applicationId, int managerId, string approvalNotes)
        {
            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Decision, StepStatus.Completed, approvalNotes, managerId);
            
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            application.Status = ApplicationStatus.Approved;
            application.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            await _applicationRepository.UpdateAsync(application);

            // Get customer and loan details for notification
            var customer = application.Applicants.FirstOrDefault()?.Customer;
            var loanDetails = await _loanDetailsRepository.GetAllAsync();
            var loanDetail = loanDetails.FirstOrDefault(ld => ld.LoanApplicationBaseId == applicationId);
            
            if (customer != null && loanDetail != null)
            {
                await _notificationService.NotifyLoanApprovedAsync(
                    customer.UserId,
                    managerId,
                    loanDetail.RequestedAmount,
                    applicationId);
            }

            return true;
        }

        public async Task<bool> RejectApplicationAsync(int applicationId, int managerId, string rejectionReason)
        {
            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Decision, StepStatus.Failed, rejectionReason, managerId);
            
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            application.Status = ApplicationStatus.Rejected;
            await _applicationRepository.UpdateAsync(application);

            // Notify customer of rejection
            var customer = application.Applicants.FirstOrDefault()?.Customer;
            if (customer != null)
            {
                await _notificationService.NotifyLoanRejectedAsync(
                    customer.UserId,
                    managerId,
                    rejectionReason,
                    applicationId);
            }

            return true;
        }

        public async Task<bool> ReviseApplicationAsync(int applicationId, LoanRevisionInputDto revisionData, int managerId)
        {
            var loanDetails = await _loanDetailsRepository.GetAllAsync();
            var loanDetail = loanDetails.FirstOrDefault(ld => ld.LoanApplicationBaseId == applicationId);
            
            if (loanDetail != null)
            {
                loanDetail.RequestedAmount = revisionData.NewApprovedAmount;
                loanDetail.TenureMonths = revisionData.NewTenureMonths;
                await _loanDetailsRepository.UpdateAsync(loanDetail);
            }

            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Decision, StepStatus.Completed, $"Revised: Amount={revisionData.NewApprovedAmount}, Tenure={revisionData.NewTenureMonths}", managerId);
            return true;
        }

        public async Task<bool> DisburseApplicationAsync(int applicationId, int managerId, decimal disbursedAmount)
        {
            try
            {
                await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Disbursed, StepStatus.InProgress, $"Disbursement initiated: {disbursedAmount}", managerId);
                
                // Get application and customer details
                var application = await _applicationRepository.GetByIdAsync(applicationId);
                var applicantCustomer = application.Applicants.FirstOrDefault()?.Customer;
                var user = applicantCustomer != null ? await _userRepository.GetByIdAsync(applicantCustomer.UserId) : null;
                var customerName = user?.FullName ?? "Customer";
                
                // Use Razorpay to transfer money to customer
                var razorpayTransactionId = await _razorpayService.TransferToCustomerAsync(applicationId, disbursedAmount, customerName);
                
                if (!string.IsNullOrEmpty(razorpayTransactionId))
                {
                    // Update application status
                    application.Status = ApplicationStatus.Disbursed;
                    await _applicationRepository.UpdateAsync(application);
                    
                    // Create loan account with Razorpay transaction ID
                    if (applicantCustomer != null)
                    {
                        var loanAccount = new LoanAccount
                        {
                            LoanApplicationBaseId = applicationId,
                            CustomerId = applicantCustomer.CustomerId,
                            CurrentPaymentStatus = LoanPaymentStatus.Active,
                            DisbursementDate = DateTime.UtcNow,
                            TotalLoanAmount = disbursedAmount,
                            PrincipalRemaining = disbursedAmount,
                            LastStatusUpdate = DateTime.UtcNow,
                            DisbursementTransactionId = razorpayTransactionId
                        };
                        await _loanAccountRepository.AddAsync(loanAccount);
                    }
                    
                    await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Disbursed, StepStatus.Completed, $"Disbursed ₹{disbursedAmount} via Razorpay: {razorpayTransactionId}", managerId);
                    
                    // Notify customer
                    if (applicantCustomer != null)
                    {
                        await _notificationService.NotifyLoanDisbursedAsync(applicantCustomer.UserId, managerId, disbursedAmount, applicationId);
                    }
                    
                    return true;
                }
                else
                {
                    await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Disbursed, StepStatus.Failed, "Razorpay transfer failed", managerId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Disbursed, StepStatus.Failed, $"Disbursement failed: {ex.Message}", managerId);
                return false;
            }
        }

        private LoanOriginationWorkflowDTO MapToWorkflowDto(LoanOriginationWorkflow workflow)
        {
            return new LoanOriginationWorkflowDTO
            {
                WorkflowId = workflow.WorkflowId,
                LoanApplicationBaseId = workflow.LoanApplicationBaseId,
                StepName = workflow.StepName.ToString(),
                StepStatus = workflow.StepStatus.ToString(),
                CompletionDate = workflow.CompletionDate,
                ManagerId = workflow.ManagerId,
                ManagerNotes = workflow.ManagerNotes
            };
        }
    }
}
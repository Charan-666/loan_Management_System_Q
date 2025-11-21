using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface INotificationService
    {
        // Basic CRUD operations
        Task<NotificationDTO> CreateNotificationAsync(NotificationDTO notificationDto);
        Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<NotificationDTO>> GetUnreadNotificationsAsync(int userId);
        Task<NotificationDTO> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);

        // Payment-related notifications
        Task NotifyPaymentSuccessAsync(int customerId, int managerId, decimal amount, string emiDetails);
        Task NotifyPaymentFailedAsync(int customerId, decimal amount, string emiDetails);
        Task NotifyLoanDisbursedAsync(int customerId, int managerId, decimal amount, int loanAccountId);
        Task NotifyDisbursementFailedAsync(int customerId, decimal amount, string reason);

        // Loan application status notifications
        Task NotifyLoanApplicationStatusAsync(int customerId, int managerId, string status, int applicationId);
        Task NotifyLoanApprovedAsync(int customerId, int managerId, decimal amount, int applicationId);
        Task NotifyLoanRejectedAsync(int customerId, int managerId, string reason, int applicationId);

        // EMI and payment due notifications
        Task NotifyEMIDueAsync(int customerId, decimal amount, DateTime dueDate, int emiId);
        Task NotifyOverduePaymentAsync(int customerId, decimal amount, int daysPastDue, int emiId);
        Task NotifyLoanFullyPaidAsync(int customerId, int managerId, int loanAccountId);

        // Manager notifications
        Task NotifyManagerMonthlyPaymentAsync(int managerId, int customerId, decimal amount, string customerName);
        Task NotifyManagerNewApplicationAsync(int managerId, int customerId, string loanType, decimal amount);
        Task NotifyManagerDocumentVerificationAsync(int managerId, int applicationId, string customerName);
        
        // Email with PDF attachment for loan application submission
        Task NotifyLoanApplicationSubmittedWithPdfAsync(int customerId, int applicationId, string loanType, decimal amount);
        

    }
}
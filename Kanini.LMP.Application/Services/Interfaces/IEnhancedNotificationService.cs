using Kanini.LMP.Database.Enums;
using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IEnhancedNotificationService
    {
        // Multi-channel notification methods
        Task SendMultiChannelNotificationAsync(int userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Medium);
        
        // Notification preference management
        Task<bool> UpdateNotificationPreferencesAsync(int userId, NotificationType type, bool email, bool sms, bool push, bool whatsApp, bool inApp);
        Task<Dictionary<NotificationType, Dictionary<NotificationChannel, bool>>> GetUserNotificationPreferencesAsync(int userId);
        
        // Enhanced loan notifications with multi-channel support
        Task NotifyLoanApplicationSubmittedAsync(int customerId, int applicationId, string loanType, decimal amount);
        Task NotifyLoanApprovedAsync(int customerId, int managerId, int applicationId, decimal amount);
        Task NotifyLoanRejectedAsync(int customerId, int managerId, int applicationId, string reason);
        Task NotifyLoanDisbursedAsync(int customerId, int managerId, decimal amount, int loanAccountId);
        
        // Enhanced payment notifications
        Task NotifyPaymentDueAsync(int customerId, decimal amount, DateTime dueDate, int emiId);
        Task NotifyPaymentSuccessAsync(int customerId, int managerId, decimal amount, string emiDetails);
        Task NotifyPaymentFailedAsync(int customerId, decimal amount, string emiDetails);
        Task NotifyOverduePaymentAsync(int customerId, decimal amount, int daysPastDue, int emiId);
        
        // Manager notifications
        Task NotifyManagerNewApplicationAsync(int managerId, int customerId, string customerName, string loanType, decimal amount);
        Task NotifyManagerDocumentVerificationAsync(int managerId, int applicationId, string customerName);
        
        // Bulk notifications
        Task SendBulkNotificationAsync(List<int> userIds, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Medium);
    }
}
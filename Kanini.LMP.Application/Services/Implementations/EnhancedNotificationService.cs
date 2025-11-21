using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using Kanini.LMP.Database.EntitiesDto;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EnhancedNotificationService : IEnhancedNotificationService
    {
        private readonly ILMPRepository<Notification, int> _notificationRepository;
        private readonly ILMPRepository<NotificationPreference, int> _preferenceRepository;
        private readonly ILMPRepository<User, int> _userRepository;
        private readonly ILMPRepository<Customer, int> _customerRepository;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private readonly IWhatsAppService _whatsAppService;

        private readonly ILogger<EnhancedNotificationService> _logger;

        public EnhancedNotificationService(
            ILMPRepository<Notification, int> notificationRepository,
            ILMPRepository<NotificationPreference, int> preferenceRepository,
            ILMPRepository<User, int> userRepository,
            ILMPRepository<Customer, int> customerRepository,
            IEmailService emailService,
            ISMSService smsService,
            IWhatsAppService whatsAppService,

            ILogger<EnhancedNotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _preferenceRepository = preferenceRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _emailService = emailService;
            _smsService = smsService;
            _whatsAppService = whatsAppService;

            _logger = logger;
        }

        public async Task SendMultiChannelNotificationAsync(int userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Medium)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return;

            var preferences = await GetUserPreferencesAsync(userId, type);
            var customer = await _customerRepository.GetAsync(c => c.UserId == userId);

            // Always create in-app notification
            if (preferences.InAppEnabled)
            {
                await CreateInAppNotificationAsync(userId, title, message, type, priority);
            }

            // Send email notification
            if (preferences.EmailEnabled && !string.IsNullOrEmpty(user.Email))
            {
                try
                {
                    await _emailService.SendEmailAsync(new EmailDto
                    {
                        ToEmail = user.Email,
                        Subject = title,
                        Body = message
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send email to {user.Email}");
                }
            }

            // Send SMS notification
            if (preferences.SMSEnabled && customer != null && !string.IsNullOrEmpty(customer.PhoneNumber))
            {
                try
                {
                    await _smsService.SendSMSAsync(customer.PhoneNumber, $"{title}: {message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send SMS to {customer.PhoneNumber}");
                }
            }

            // Send WhatsApp notification
            if (preferences.WhatsAppEnabled && customer != null && !string.IsNullOrEmpty(customer.PhoneNumber))
            {
                try
                {
                    await _whatsAppService.SendWhatsAppMessageAsync(customer.PhoneNumber, $"*{title}*\n\n{message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send WhatsApp to {customer.PhoneNumber}");
                }
            }


        }

        public async Task<bool> UpdateNotificationPreferencesAsync(int userId, NotificationType type, bool email, bool sms, bool push, bool whatsApp, bool inApp)
        {
            var preference = await _preferenceRepository.GetAsync(p => p.UserId == userId && p.Type == type);
            
            if (preference == null)
            {
                preference = new NotificationPreference
                {
                    UserId = userId,
                    Type = type,
                    EmailEnabled = email,
                    SMSEnabled = sms,
                    PushEnabled = push,
                    WhatsAppEnabled = whatsApp,
                    InAppEnabled = inApp
                };
                await _preferenceRepository.AddAsync(preference);
            }
            else
            {
                preference.EmailEnabled = email;
                preference.SMSEnabled = sms;
                preference.PushEnabled = push;
                preference.WhatsAppEnabled = whatsApp;
                preference.InAppEnabled = inApp;
                await _preferenceRepository.UpdateAsync(preference);
            }

            return true;
        }

        public async Task<Dictionary<NotificationType, Dictionary<NotificationChannel, bool>>> GetUserNotificationPreferencesAsync(int userId)
        {
            var preferences = await _preferenceRepository.GetAllAsync(p => p.UserId == userId);
            var result = new Dictionary<NotificationType, Dictionary<NotificationChannel, bool>>();

            foreach (NotificationType type in Enum.GetValues<NotificationType>())
            {
                var pref = preferences.FirstOrDefault(p => p.Type == type);
                result[type] = new Dictionary<NotificationChannel, bool>
                {
                    { NotificationChannel.Email, pref?.EmailEnabled ?? true },
                    { NotificationChannel.SMS, pref?.SMSEnabled ?? true },
                    { NotificationChannel.Push, pref?.PushEnabled ?? false },
                    { NotificationChannel.WhatsApp, pref?.WhatsAppEnabled ?? false },
                    { NotificationChannel.InApp, pref?.InAppEnabled ?? true }
                };
            }

            return result;
        }

        public async Task NotifyLoanApplicationSubmittedAsync(int customerId, int applicationId, string loanType, decimal amount)
        {
            var title = "Loan Application Submitted";
            var message = $"Your {loanType} application #{applicationId} for ₹{amount:N0} has been submitted successfully. We will review and update you soon.";
            
            await SendMultiChannelNotificationAsync(customerId, title, message, NotificationType.LoanApplication, NotificationPriority.Medium);
        }

        public async Task NotifyLoanApprovedAsync(int customerId, int managerId, int applicationId, decimal amount)
        {
            var title = "🎉 Loan Approved!";
            var message = $"Congratulations! Your loan application #{applicationId} for ₹{amount:N0} has been approved. Disbursement will be processed soon.";
            
            await SendMultiChannelNotificationAsync(customerId, title, message, NotificationType.LoanApproved, NotificationPriority.High);
            
            // Notify manager
            var managerTitle = "Loan Approval Processed";
            var managerMessage = $"Loan application #{applicationId} for ₹{amount:N0} has been approved and processed.";
            await SendMultiChannelNotificationAsync(managerId, managerTitle, managerMessage, NotificationType.General, NotificationPriority.Medium);
        }

        public async Task NotifyLoanRejectedAsync(int customerId, int managerId, int applicationId, string reason)
        {
            var title = "Loan Application Update";
            var message = $"We regret to inform that your loan application #{applicationId} has been rejected. Reason: {reason}. Please contact us for more details.";
            
            await SendMultiChannelNotificationAsync(customerId, title, message, NotificationType.LoanRejected, NotificationPriority.High);
        }

        public async Task NotifyLoanDisbursedAsync(int customerId, int managerId, decimal amount, int loanAccountId)
        {
            var title = "💰 Loan Disbursed Successfully";
            var message = $"Great news! Your loan amount of ₹{amount:N0} has been disbursed to your registered bank account. Loan Account: {loanAccountId}";
            
            await SendMultiChannelNotificationAsync(customerId, title, message, NotificationType.LoanDisbursed, NotificationPriority.High);
            
            // Notify manager
            var managerTitle = "Loan Disbursement Completed";
            var managerMessage = $"Loan disbursement of ₹{amount:N0} completed for Loan Account: {loanAccountId}";
            await SendMultiChannelNotificationAsync(managerId, managerTitle, managerMessage, NotificationType.General, NotificationPriority.Medium);
        }

        public async Task NotifyPaymentDueAsync(int customerId, decimal amount, DateTime dueDate, int emiId)
        {
            var daysUntilDue = (dueDate - DateTime.Now).Days;
            var title = "EMI Payment Due";
            var message = daysUntilDue <= 0 
                ? $"Your EMI payment of ₹{amount:N0} is due today. Please make the payment to avoid late fees."
                : $"Your EMI payment of ₹{amount:N0} is due in {daysUntilDue} days on {dueDate:dd MMM yyyy}.";
            
            var priority = daysUntilDue <= 1 ? NotificationPriority.High : NotificationPriority.Medium;
            await SendMultiChannelNotificationAsync(customerId, title, message, NotificationType.PaymentDue, priority);
        }

        public async Task NotifyPaymentSuccessAsync(int customerId, int managerId, decimal amount, string emiDetails)
        {
            var title = "✅ Payment Successful";
            var message = $"Your EMI payment of ₹{amount:N0} has been processed successfully. {emiDetails}";
            
            await SendMultiChannelNotificationAsync(customerId, title, message, NotificationType.PaymentSuccess, NotificationPriority.Medium);
            
            // Notify manager
            var managerTitle = "EMI Payment Received";
            var managerMessage = $"EMI payment of ₹{amount:N0} received from customer. {emiDetails}";
            await SendMultiChannelNotificationAsync(managerId, managerTitle, managerMessage, NotificationType.General, NotificationPriority.Low);
        }

        public async Task NotifyPaymentFailedAsync(int customerId, decimal amount, string emiDetails)
        {
            var title = "❌ Payment Failed";
            var message = $"Your EMI payment of ₹{amount:N0} could not be processed. Please try again or contact support. {emiDetails}";
            
            await SendMultiChannelNotificationAsync(customerId, title, message, NotificationType.PaymentFailed, NotificationPriority.High);
        }

        public async Task NotifyOverduePaymentAsync(int customerId, decimal amount, int daysPastDue, int emiId)
        {
            var title = "🚨 Overdue Payment Alert";
            var message = $"URGENT: Your EMI payment of ₹{amount:N0} is {daysPastDue} days overdue. Please make the payment immediately to avoid penalties.";
            
            await SendMultiChannelNotificationAsync(customerId, title, message, NotificationType.PaymentDue, NotificationPriority.Critical);
        }

        public async Task NotifyManagerNewApplicationAsync(int managerId, int customerId, string customerName, string loanType, decimal amount)
        {
            var title = "📋 New Loan Application";
            var message = $"New {loanType} application for ₹{amount:N0} submitted by {customerName} (Customer ID: {customerId}). Review required.";
            
            await SendMultiChannelNotificationAsync(managerId, title, message, NotificationType.LoanApplication, NotificationPriority.Medium);
        }

        public async Task NotifyManagerDocumentVerificationAsync(int managerId, int applicationId, string customerName)
        {
            var title = "📄 Document Verification Required";
            var message = $"Document verification required for application #{applicationId} from {customerName}.";
            
            await SendMultiChannelNotificationAsync(managerId, title, message, NotificationType.DocumentVerification, NotificationPriority.Medium);
        }

        public async Task SendBulkNotificationAsync(List<int> userIds, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Medium)
        {
            var tasks = userIds.Select(userId => SendMultiChannelNotificationAsync(userId, title, message, type, priority));
            await Task.WhenAll(tasks);
        }

        private async Task<NotificationPreference> GetUserPreferencesAsync(int userId, NotificationType type)
        {
            var preference = await _preferenceRepository.GetAsync(p => p.UserId == userId && p.Type == type);
            return preference ?? new NotificationPreference
            {
                UserId = userId,
                Type = type,
                EmailEnabled = true,
                SMSEnabled = true,
                PushEnabled = false, // Not used in web app
                WhatsAppEnabled = false,
                InAppEnabled = true
            };
        }

        private async Task CreateInAppNotificationAsync(int userId, string title, string message, NotificationType type, NotificationPriority priority)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Channel = NotificationChannel.InApp,
                Type = type,
                Priority = priority,
                IsRead = false,
                IsSent = true,
                CreatedAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
        }
    }
}
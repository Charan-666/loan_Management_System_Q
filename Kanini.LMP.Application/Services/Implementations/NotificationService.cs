using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly ILMPRepository<Notification, int> _notificationRepository;
        private readonly ILMPRepository<User, int> _userRepository;
        private readonly IEmailService _emailService;
        private readonly IPdfService _pdfService;

        public NotificationService(
            ILMPRepository<Notification, int> notificationRepository,
            ILMPRepository<User, int> userRepository,
            IEmailService emailService,
            IPdfService pdfService)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        public async Task<NotificationDTO> CreateNotificationAsync(NotificationDTO notificationDto)
        {
            var notification = new Notification
            {
                UserId = notificationDto.UserId,
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _notificationRepository.AddAsync(notification);
            return MapToDto(created);
        }

        public async Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetAllAsync(n => n.UserId == userId);
            return notifications.OrderByDescending(n => n.CreatedAt).Select(MapToDto);
        }

        public async Task<IEnumerable<NotificationDTO>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetAllAsync(n => n.UserId == userId && !n.IsRead);
            return notifications.OrderByDescending(n => n.CreatedAt).Select(MapToDto);
        }

        public async Task<NotificationDTO> MarkAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null) throw new ArgumentException("Notification not found");

            notification.IsRead = true;
            var updated = await _notificationRepository.UpdateAsync(notification);
            return MapToDto(updated);
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _notificationRepository.GetAllAsync(n => n.UserId == userId && !n.IsRead);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
            }
            return true;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            var notifications = await _notificationRepository.GetAllAsync(n => n.UserId == userId && !n.IsRead);
            return notifications.Count;
        }

        // Payment-related notifications
        public async Task NotifyPaymentSuccessAsync(int customerId, int managerId, decimal amount, string emiDetails)
        {
            // Get customer details for email
            var customer = await _userRepository.GetByIdAsync(customerId);
            
            // Notify customer
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Payment Successful",
                Message = $"Your EMI payment of ₹{amount:N2} has been processed successfully. {emiDetails}"
            });

            // Send email to customer
            if (customer != null && !string.IsNullOrEmpty(customer.Email))
            {
                await _emailService.SendPaymentSuccessEmailAsync(
                    customer.Email,
                    customer.FullName ?? "Customer",
                    amount,
                    emiDetails,
                    DateTime.Now);
            }

            // Notify manager
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = managerId,
                Title = "EMI Payment Received",
                Message = $"EMI payment of ₹{amount:N2} received from customer. {emiDetails}"
            });
        }

        public async Task NotifyPaymentFailedAsync(int customerId, decimal amount, string emiDetails)
        {
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Payment Failed",
                Message = $"Your EMI payment of ₹{amount:N2} could not be processed. Please try again. {emiDetails}"
            });
        }

        public async Task NotifyLoanDisbursedAsync(int customerId, int managerId, decimal amount, int loanAccountId)
        {
            // Notify customer
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Loan Disbursed Successfully",
                Message = $"Your loan amount of ₹{amount:N2} has been disbursed to your account. Loan Account: {loanAccountId}"
            });

            // Notify manager
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = managerId,
                Title = "Loan Disbursement Completed",
                Message = $"Loan disbursement of ₹{amount:N2} completed for Loan Account: {loanAccountId}"
            });
        }

        public async Task NotifyDisbursementFailedAsync(int customerId, decimal amount, string reason)
        {
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Loan Disbursement Failed",
                Message = $"Disbursement of ₹{amount:N2} failed. Reason: {reason}. Please contact support."
            });
        }

        // Loan application status notifications
        public async Task NotifyLoanApplicationStatusAsync(int customerId, int managerId, string status, int applicationId)
        {
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Loan Application Update",
                Message = $"Your loan application #{applicationId} status has been updated to: {status}"
            });
        }

        public async Task NotifyLoanApprovedAsync(int customerId, int managerId, decimal amount, int applicationId)
        {
            // Get customer details
            var customer = await _userRepository.GetByIdAsync(customerId);
            
            // Notify customer
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Loan Approved!",
                Message = $"Congratulations! Your loan application #{applicationId} for ₹{amount:N2} has been approved."
            });

            // Send email to customer
            if (customer != null && !string.IsNullOrEmpty(customer.Email))
            {
                await _emailService.SendLoanApprovedEmailAsync(
                    customer.Email,
                    customer.FullName ?? "Customer",
                    applicationId,
                    amount,
                    "Personal Loan"); // You may need to get actual loan type
            }

            // Notify manager
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = managerId,
                Title = "Loan Approval Processed",
                Message = $"Loan application #{applicationId} for ₹{amount:N2} has been approved and processed."
            });
        }

        public async Task NotifyLoanRejectedAsync(int customerId, int managerId, string reason, int applicationId)
        {
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Loan Application Rejected",
                Message = $"Your loan application #{applicationId} has been rejected. Reason: {reason}"
            });
        }

        // EMI and payment due notifications
        public async Task NotifyEMIDueAsync(int customerId, decimal amount, DateTime dueDate, int emiId)
        {
            var daysUntilDue = (dueDate - DateTime.Now).Days;
            var message = daysUntilDue <= 0 
                ? $"Your EMI payment of ₹{amount:N2} is due today. Please make the payment to avoid late fees."
                : $"Your EMI payment of ₹{amount:N2} is due in {daysUntilDue} days on {dueDate:dd MMM yyyy}.";

            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "EMI Payment Due",
                Message = message
            });
        }

        public async Task NotifyOverduePaymentAsync(int customerId, decimal amount, int daysPastDue, int emiId)
        {
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Overdue Payment Alert",
                Message = $"Your EMI payment of ₹{amount:N2} is {daysPastDue} days overdue. Please make the payment immediately to avoid penalties."
            });
        }

        public async Task NotifyLoanFullyPaidAsync(int customerId, int managerId, int loanAccountId)
        {
            // Notify customer
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Loan Fully Paid!",
                Message = $"Congratulations! You have successfully completed all payments for Loan Account: {loanAccountId}."
            });

            // Notify manager
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = managerId,
                Title = "Loan Account Closed",
                Message = $"Loan Account: {loanAccountId} has been fully paid and closed successfully."
            });
        }

        // Manager notifications
        public async Task NotifyManagerMonthlyPaymentAsync(int managerId, int customerId, decimal amount, string customerName)
        {
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = managerId,
                Title = "Monthly Payment Received",
                Message = $"Monthly EMI payment of ₹{amount:N2} received from {customerName} (Customer ID: {customerId})."
            });
        }

        public async Task NotifyManagerNewApplicationAsync(int managerId, int customerId, string loanType, decimal amount)
        {
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = managerId,
                Title = "New Loan Application",
                Message = $"New {loanType} application for ₹{amount:N2} submitted by Customer ID: {customerId}. Review required."
            });
        }

        public async Task NotifyManagerDocumentVerificationAsync(int managerId, int applicationId, string customerName)
        {
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = managerId,
                Title = "Document Verification Required",
                Message = $"Document verification required for application #{applicationId} from {customerName}."
            });
        }

        public async Task NotifyLoanApplicationSubmittedWithPdfAsync(int customerId, int applicationId, string loanType, decimal amount)
        {
            // Get customer details
            var customer = await _userRepository.GetByIdAsync(customerId);
            
            // Create notification
            await CreateNotificationAsync(new NotificationDTO
            {
                UserId = customerId,
                Title = "Loan Application Submitted",
                Message = $"Your {loanType} application #{applicationId} for ₹{amount:N2} has been submitted successfully."
            });

            // Generate PDF and send email
            if (customer != null && !string.IsNullOrEmpty(customer.Email))
            {
                var applicationPdf = await _pdfService.GenerateLoanApplicationPdfAsync(applicationId);
                await _emailService.SendLoanApplicationSubmittedEmailAsync(
                    customer.Email,
                    customer.FullName ?? "Customer",
                    applicationId,
                    loanType,
                    amount,
                    applicationPdf);
            }
        }

        private NotificationDTO MapToDto(Notification notification)
        {
            return new NotificationDTO
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
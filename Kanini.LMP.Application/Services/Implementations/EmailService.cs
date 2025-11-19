using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Database.EntitiesDto.Email;
using System.Net;
using System.Net.Mail;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                
                using var client = new SmtpClient(smtpSettings["Host"] ?? "smtp.gmail.com", int.Parse(smtpSettings["Port"] ?? "587"))
                {
                    Credentials = new NetworkCredential(smtpSettings["Username"] ?? "", smtpSettings["Password"] ?? ""),
                    EnableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true")
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"] ?? "noreply@lmp.com", smtpSettings["FromName"] ?? "LMP"),
                    Subject = emailDto.Subject,
                    Body = emailDto.Body,
                    IsBodyHtml = emailDto.IsHtml
                };

                message.To.Add(new MailAddress(emailDto.ToEmail, emailDto.ToName));

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {emailDto.ToEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {emailDto.ToEmail}");
                return false;
            }
        }

        public async Task<bool> SendLoanApplicationSubmittedEmailAsync(string customerEmail, string customerName, int applicationId, string loanType, decimal amount, byte[] applicationPdf)
        {
            var subject = $"Loan Application Submitted - Application #{applicationId}";
            var body = $"Dear {customerName},\n\nYour {loanType} application for ₹{amount:N2} has been submitted successfully.\n\nApplication ID: {applicationId}\n\nWe will review and get back to you within 3-5 business days.\n\nBest regards,\nLMP Team";
            
            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = subject,
                Body = body
            };
            
            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendLoanApprovedEmailAsync(string customerEmail, string customerName, int applicationId, decimal amount, string loanType)
        {
            var subject = string.Format(EmailTemplates.LoanApprovedSubject, applicationId);
            var body = string.Format(EmailTemplates.LoanApprovedBody, customerName, applicationId, loanType, amount, DateTime.Now.ToString("dd MMM yyyy"));
            
            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = subject,
                Body = body
            };
            
            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendPaymentSuccessEmailAsync(string customerEmail, string customerName, decimal amount, string emiDetails, DateTime paymentDate)
        {
            var body = string.Format(EmailTemplates.PaymentSuccessBody, customerName, amount, paymentDate.ToString("dd MMM yyyy HH:mm"));
            
            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = EmailTemplates.PaymentSuccessSubject,
                Body = body
            };
            
            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendLoanRejectedEmailAsync(string customerEmail, string customerName, int applicationId, string reason)
        {
            var subject = $"Loan Application Update - Application #{applicationId}";
            var body = $"Dear {customerName},\n\nWe regret to inform you that your loan application #{applicationId} has been rejected.\n\nReason: {reason}\n\nYou may reapply after addressing the concerns.\n\nBest regards,\nLMP Team";
            
            var emailDto = new EmailDto { ToEmail = customerEmail, ToName = customerName, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendPaymentFailedEmailAsync(string customerEmail, string customerName, decimal amount, string emiDetails, string reason)
        {
            var subject = "❌ Payment Failed - Action Required";
            var body = $"Dear {customerName},\n\nYour EMI payment of ₹{amount:N2} failed.\n\nReason: {reason}\n\nPlease try again.\n\nBest regards,\nLMP Team";
            
            var emailDto = new EmailDto { ToEmail = customerEmail, ToName = customerName, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendEMIDueReminderEmailAsync(string customerEmail, string customerName, decimal amount, DateTime dueDate, int daysUntilDue)
        {
            var subject = $"📅 EMI Payment Reminder - Due {dueDate:dd MMM yyyy}";
            var urgency = daysUntilDue <= 0 ? "is due today" : $"is due in {daysUntilDue} days";
            var body = $"Dear {customerName},\n\nYour EMI payment of ₹{amount:N2} {urgency}.\n\nDue Date: {dueDate:dd MMM yyyy}\n\nBest regards,\nLMP Team";
            
            var emailDto = new EmailDto { ToEmail = customerEmail, ToName = customerName, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendOverduePaymentEmailAsync(string customerEmail, string customerName, decimal amount, int daysPastDue)
        {
            var subject = $"⚠️ Urgent: Overdue Payment - {daysPastDue} Days Past Due";
            var body = $"Dear {customerName},\n\nYour EMI payment of ₹{amount:N2} is {daysPastDue} days overdue.\n\nPlease make payment immediately.\n\nBest regards,\nLMP Team";
            
            var emailDto = new EmailDto { ToEmail = customerEmail, ToName = customerName, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendLoanDisbursedEmailAsync(string customerEmail, string customerName, decimal amount, int loanAccountId, DateTime disbursementDate)
        {
            var subject = "💰 Loan Disbursed Successfully";
            var body = $"Dear {customerName},\n\nYour loan of ₹{amount:N2} has been disbursed successfully.\n\nLoan Account ID: {loanAccountId}\n\nDisbursement Date: {disbursementDate:dd MMM yyyy}\n\nBest regards,\nLMP Team";
            
            var emailDto = new EmailDto { ToEmail = customerEmail, ToName = customerName, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendLoanFullyPaidEmailAsync(string customerEmail, string customerName, int loanAccountId, decimal totalAmountPaid)
        {
            var subject = "🎉 Congratulations! Loan Fully Paid";
            var body = $"Dear {customerName},\n\nCongratulations! You have fully paid your loan.\n\nLoan Account ID: {loanAccountId}\n\nTotal Amount Paid: ₹{totalAmountPaid:N2}\n\nBest regards,\nLMP Team";
            
            var emailDto = new EmailDto { ToEmail = customerEmail, ToName = customerName, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }
    }
}
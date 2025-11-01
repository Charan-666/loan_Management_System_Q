using Kanini.LMP.Database.EntitiesDto.Email;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailDto emailDto);
        Task<bool> SendLoanApplicationSubmittedEmailAsync(string customerEmail, string customerName, int applicationId, string loanType, decimal amount, byte[] applicationPdf);
        Task<bool> SendLoanApprovedEmailAsync(string customerEmail, string customerName, int applicationId, decimal amount, string loanType);
        Task<bool> SendLoanRejectedEmailAsync(string customerEmail, string customerName, int applicationId, string reason);
        Task<bool> SendPaymentSuccessEmailAsync(string customerEmail, string customerName, decimal amount, string emiDetails, DateTime paymentDate);
        Task<bool> SendPaymentFailedEmailAsync(string customerEmail, string customerName, decimal amount, string emiDetails, string reason);
        Task<bool> SendEMIDueReminderEmailAsync(string customerEmail, string customerName, decimal amount, DateTime dueDate, int daysUntilDue);
        Task<bool> SendOverduePaymentEmailAsync(string customerEmail, string customerName, decimal amount, int daysPastDue);
        Task<bool> SendLoanDisbursedEmailAsync(string customerEmail, string customerName, decimal amount, int loanAccountId, DateTime disbursementDate);
        Task<bool> SendLoanFullyPaidEmailAsync(string customerEmail, string customerName, int loanAccountId, decimal totalAmountPaid);
    }
}
namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IWhatsAppService
    {
        Task<bool> SendWhatsAppMessageAsync(string phoneNumber, string message);
        Task<bool> SendLoanApplicationWhatsAppAsync(string phoneNumber, string customerName, int applicationId, string loanType);
        Task<bool> SendLoanApprovedWhatsAppAsync(string phoneNumber, string customerName, int applicationId, decimal amount);
        Task<bool> SendPaymentDueWhatsAppAsync(string phoneNumber, string customerName, decimal amount, DateTime dueDate);
        Task<bool> SendPaymentSuccessWhatsAppAsync(string phoneNumber, string customerName, decimal amount);
        Task<bool> SendLoanDisbursedWhatsAppAsync(string phoneNumber, string customerName, decimal amount);
    }
}
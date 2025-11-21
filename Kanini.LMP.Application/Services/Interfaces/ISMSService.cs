namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ISMSService
    {
        Task<bool> SendSMSAsync(string phoneNumber, string message);
        Task<bool> SendLoanApplicationSMSAsync(string phoneNumber, string customerName, int applicationId, string loanType);
        Task<bool> SendLoanApprovedSMSAsync(string phoneNumber, string customerName, int applicationId, decimal amount);
        Task<bool> SendLoanRejectedSMSAsync(string phoneNumber, string customerName, int applicationId);
        Task<bool> SendPaymentDueSMSAsync(string phoneNumber, string customerName, decimal amount, DateTime dueDate);
        Task<bool> SendPaymentSuccessSMSAsync(string phoneNumber, string customerName, decimal amount);
        Task<bool> SendOverduePaymentSMSAsync(string phoneNumber, string customerName, decimal amount, int daysPastDue);
        Task<bool> SendLoanDisbursedSMSAsync(string phoneNumber, string customerName, decimal amount);
    }
}
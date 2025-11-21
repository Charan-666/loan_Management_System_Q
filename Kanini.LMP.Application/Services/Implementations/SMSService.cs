using Kanini.LMP.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class SMSService : ISMSService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SMSService> _logger;
        private readonly string _apiKey;
        private readonly string _senderId;
        private readonly string _baseUrl;

        public SMSService(HttpClient httpClient, IConfiguration configuration, ILogger<SMSService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["SMS:ApiKey"] ?? "";
            _senderId = _configuration["SMS:SenderId"] ?? "LMPAPP";
            _baseUrl = _configuration["SMS:BaseUrl"] ?? "https://api.textlocal.in/send/";
        }

        public async Task<bool> SendSMSAsync(string phoneNumber, string message)
        {
            try
            {
                var payload = new
                {
                    apikey = _apiKey,
                    numbers = phoneNumber,
                    message = message,
                    sender = _senderId
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"SMS sent to {phoneNumber}: {response.IsSuccessStatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SMS to {phoneNumber}");
                return false;
            }
        }

        public async Task<bool> SendLoanApplicationSMSAsync(string phoneNumber, string customerName, int applicationId, string loanType)
        {
            var message = $"Dear {customerName}, your {loanType} application #{applicationId} has been submitted successfully. We will review and update you soon. - LMP";
            return await SendSMSAsync(phoneNumber, message);
        }

        public async Task<bool> SendLoanApprovedSMSAsync(string phoneNumber, string customerName, int applicationId, decimal amount)
        {
            var message = $"Congratulations {customerName}! Your loan application #{applicationId} for Rs.{amount:N0} has been approved. Disbursement will be processed soon. - LMP";
            return await SendSMSAsync(phoneNumber, message);
        }

        public async Task<bool> SendLoanRejectedSMSAsync(string phoneNumber, string customerName, int applicationId)
        {
            var message = $"Dear {customerName}, we regret to inform that your loan application #{applicationId} has been rejected. Please contact us for more details. - LMP";
            return await SendSMSAsync(phoneNumber, message);
        }

        public async Task<bool> SendPaymentDueSMSAsync(string phoneNumber, string customerName, decimal amount, DateTime dueDate)
        {
            var message = $"Dear {customerName}, your EMI of Rs.{amount:N0} is due on {dueDate:dd-MMM-yyyy}. Please make payment to avoid late fees. - LMP";
            return await SendSMSAsync(phoneNumber, message);
        }

        public async Task<bool> SendPaymentSuccessSMSAsync(string phoneNumber, string customerName, decimal amount)
        {
            var message = $"Dear {customerName}, your EMI payment of Rs.{amount:N0} has been received successfully. Thank you! - LMP";
            return await SendSMSAsync(phoneNumber, message);
        }

        public async Task<bool> SendOverduePaymentSMSAsync(string phoneNumber, string customerName, decimal amount, int daysPastDue)
        {
            var message = $"URGENT: Dear {customerName}, your EMI of Rs.{amount:N0} is {daysPastDue} days overdue. Please pay immediately to avoid penalties. - LMP";
            return await SendSMSAsync(phoneNumber, message);
        }

        public async Task<bool> SendLoanDisbursedSMSAsync(string phoneNumber, string customerName, decimal amount)
        {
            var message = $"Dear {customerName}, your loan amount of Rs.{amount:N0} has been disbursed to your account. Check your bank statement. - LMP";
            return await SendSMSAsync(phoneNumber, message);
        }
    }
}
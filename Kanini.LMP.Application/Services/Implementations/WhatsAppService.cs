using Kanini.LMP.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly string _accessToken;
        private readonly string _phoneNumberId;
        private readonly string _baseUrl;

        public WhatsAppService(HttpClient httpClient, IConfiguration configuration, ILogger<WhatsAppService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _accessToken = _configuration["WhatsApp:AccessToken"] ?? "";
            _phoneNumberId = _configuration["WhatsApp:PhoneNumberId"] ?? "";
            _baseUrl = $"https://graph.facebook.com/v18.0/{_phoneNumberId}/messages";
        }

        public async Task<bool> SendWhatsAppMessageAsync(string phoneNumber, string message)
        {
            try
            {
                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = phoneNumber,
                    type = "text",
                    text = new { body = message }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _httpClient.PostAsync(_baseUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"WhatsApp message sent to {phoneNumber}: {response.IsSuccessStatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send WhatsApp message to {phoneNumber}");
                return false;
            }
        }

        public async Task<bool> SendLoanApplicationWhatsAppAsync(string phoneNumber, string customerName, int applicationId, string loanType)
        {
            var message = $"🏦 *Loan Application Submitted*\n\nDear {customerName},\n\nYour {loanType} application #{applicationId} has been submitted successfully.\n\nWe will review your application and update you soon.\n\nThank you for choosing LMP! 🙏";
            return await SendWhatsAppMessageAsync(phoneNumber, message);
        }

        public async Task<bool> SendLoanApprovedWhatsAppAsync(string phoneNumber, string customerName, int applicationId, decimal amount)
        {
            var message = $"🎉 *Loan Approved!*\n\nCongratulations {customerName}!\n\nYour loan application #{applicationId} for ₹{amount:N0} has been *APPROVED*.\n\nDisbursement will be processed within 2-3 business days.\n\nThank you for choosing LMP! 🏦";
            return await SendWhatsAppMessageAsync(phoneNumber, message);
        }

        public async Task<bool> SendPaymentDueWhatsAppAsync(string phoneNumber, string customerName, decimal amount, DateTime dueDate)
        {
            var message = $"💳 *EMI Payment Due*\n\nDear {customerName},\n\nYour EMI payment of ₹{amount:N0} is due on {dueDate:dd-MMM-yyyy}.\n\nPlease make the payment on time to avoid late fees.\n\nPay now through our app or website. 📱";
            return await SendWhatsAppMessageAsync(phoneNumber, message);
        }

        public async Task<bool> SendPaymentSuccessWhatsAppAsync(string phoneNumber, string customerName, decimal amount)
        {
            var message = $"✅ *Payment Successful*\n\nDear {customerName},\n\nYour EMI payment of ₹{amount:N0} has been received successfully.\n\nThank you for your timely payment! 🙏\n\n- LMP Team";
            return await SendWhatsAppMessageAsync(phoneNumber, message);
        }

        public async Task<bool> SendLoanDisbursedWhatsAppAsync(string phoneNumber, string customerName, decimal amount)
        {
            var message = $"💰 *Loan Disbursed*\n\nDear {customerName},\n\nGreat news! Your loan amount of ₹{amount:N0} has been disbursed to your registered bank account.\n\nPlease check your bank statement.\n\nThank you for choosing LMP! 🏦";
            return await SendWhatsAppMessageAsync(phoneNumber, message);
        }
    }
}
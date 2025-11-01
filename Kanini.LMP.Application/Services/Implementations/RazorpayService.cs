using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class RazorpayService : IRazorpayService
    {
        private readonly HttpClient _httpClient;
        private readonly string _keyId;
        private readonly string _keySecret;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;

        public RazorpayService(HttpClient httpClient, IConfiguration configuration, IPaymentService paymentService, INotificationService notificationService)
        {
            _httpClient = httpClient;
            _keyId = configuration["Razorpay:TestKeyId"] ?? "rzp_test_your_key_id";
            _keySecret = configuration["Razorpay:TestKeySecret"] ?? "your_test_key_secret";
            _paymentService = paymentService;
            _notificationService = notificationService;
            
            // Setup Razorpay Test API base URL
            _httpClient.BaseAddress = new Uri("https://api.razorpay.com/v1/");
            
            // Basic Auth for Razorpay API
            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_keyId}:{_keySecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
        }

        public async Task<RazorpayOrderResponseDto> CreateOrderAsync(RazorpayOrderCreateDto orderDto)
        {
            var orderRequest = new
            {
                amount = (int)(orderDto.Amount * 100), // Convert to paise
                currency = orderDto.Currency,
                receipt = orderDto.Receipt,
                notes = new
                {
                    loan_account_id = orderDto.LoanAccountId.ToString(),
                    emi_id = orderDto.EMIId.ToString()
                }
            };

            var json = JsonSerializer.Serialize(orderRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("orders", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var orderResponse = JsonSerializer.Deserialize<JsonElement>(responseJson);
                return new RazorpayOrderResponseDto
                {
                    Id = orderResponse.GetProperty("id").GetString() ?? "",
                    Entity = orderResponse.GetProperty("entity").GetString() ?? "",
                    Amount = orderResponse.GetProperty("amount").GetInt32() / 100m,
                    Currency = orderResponse.GetProperty("currency").GetString() ?? "",
                    Receipt = orderResponse.GetProperty("receipt").GetString() ?? "",
                    Status = orderResponse.GetProperty("status").GetString() ?? "",
                    CreatedAt = orderResponse.GetProperty("created_at").GetInt64()
                };
            }

            throw new Exception($"Failed to create Razorpay order: {responseJson}");
        }

        public async Task<PaymentTransactionResponseDTO> ProcessPaymentAsync(RazorpayPaymentDto paymentDto)
        {
            // Verify payment signature first
            var isValidSignature = await VerifyPaymentSignatureAsync(
                paymentDto.RazorpayOrderId, 
                paymentDto.RazorpayPaymentId, 
                paymentDto.RazorpaySignature);

            if (!isValidSignature)
            {
                // Notify customer of failed payment
                await _notificationService.NotifyPaymentFailedAsync(
                    paymentDto.LoanAccountId, // Assuming this maps to customer
                    0, 
                    "Invalid payment signature");
                    
                return new PaymentTransactionResponseDTO
                {
                    Status = PaymentStatus.Failed,
                    Message = "Invalid payment signature"
                };
            }

            // Fetch payment details from Razorpay
            var paymentResponse = await _httpClient.GetAsync($"payments/{paymentDto.RazorpayPaymentId}");
            var paymentJson = await paymentResponse.Content.ReadAsStringAsync();

            if (paymentResponse.IsSuccessStatusCode)
            {
                var payment = JsonSerializer.Deserialize<JsonElement>(paymentJson);
                var status = payment.GetProperty("status").GetString();
                var amount = payment.GetProperty("amount").GetInt32() / 100m;
                var method = payment.GetProperty("method").GetString();

                // Create payment record in database
                var paymentCreateDto = new PaymentTransactionCreateDTO
                {
                    EMIId = paymentDto.EMIId,
                    LoanAccountId = paymentDto.LoanAccountId,
                    Amount = amount,
                    PaymentMethod = MapRazorpayMethodToEnum(method ?? ""),
                    TransactionReference = paymentDto.RazorpayPaymentId
                };

                var createdPayment = await _paymentService.CreatePaymentAsync(paymentCreateDto);

                // Update status based on Razorpay response
                var finalStatus = status == "captured" ? 
                    Database.Entities.PaymentStatus.Success : 
                    Database.Entities.PaymentStatus.Failed;

                await _paymentService.UpdatePaymentStatusAsync(createdPayment.TransactionId, finalStatus);

                // Send notifications based on payment status
                if (status == "captured")
                {
                    await _notificationService.NotifyPaymentSuccessAsync(
                        paymentDto.LoanAccountId, // Customer ID (assuming mapping)
                        3, // Manager ID (you may need to get this dynamically)
                        amount,
                        $"EMI #{paymentDto.EMIId}");
                }
                else
                {
                    await _notificationService.NotifyPaymentFailedAsync(
                        paymentDto.LoanAccountId,
                        amount,
                        $"EMI #{paymentDto.EMIId} - Payment not captured");
                }

                return new PaymentTransactionResponseDTO
                {
                    TransactionId = createdPayment.TransactionId,
                    EMIId = paymentDto.EMIId,
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    Status = status == "captured" ? PaymentStatus.Success : PaymentStatus.Failed,
                    TransactionReference = paymentDto.RazorpayPaymentId,
                    Message = status == "captured" ? "Payment successful" : "Payment failed"
                };
            }

            return new PaymentTransactionResponseDTO
            {
                Status = PaymentStatus.Failed,
                Message = "Failed to fetch payment details from Razorpay"
            };
        }

        public async Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature)
        {
            var payload = $"{orderId}|{paymentId}";
            var expectedSignature = ComputeHmacSha256(payload, _keySecret);
            return signature == expectedSignature;
        }

        public async Task<DisbursementResponseDto> CreateDisbursementAsync(DisbursementDto disbursementDto)
        {
            var disbursementRequest = new
            {
                account_number = disbursementDto.AccountNumber,
                fund_account = new
                {
                    account_type = "bank_account",
                    bank_account = new
                    {
                        name = disbursementDto.BeneficiaryName,
                        ifsc = disbursementDto.IfscCode,
                        account_number = disbursementDto.AccountNumber
                    }
                },
                amount = (int)(disbursementDto.Amount * 100), // Convert to paise
                currency = "INR",
                mode = "IMPS",
                purpose = disbursementDto.Purpose,
                queue_if_low_balance = true,
                reference_id = $"loan_{disbursementDto.LoanAccountId}_{DateTime.Now:yyyyMMddHHmmss}",
                narration = $"Loan disbursement for account {disbursementDto.LoanAccountId}"
            };

            var json = JsonSerializer.Serialize(disbursementRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("payouts", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var disbursement = JsonSerializer.Deserialize<JsonElement>(responseJson);
                return new DisbursementResponseDto
                {
                    Id = disbursement.GetProperty("id").GetString() ?? "",
                    Entity = disbursement.GetProperty("entity").GetString() ?? "",
                    Amount = disbursement.GetProperty("amount").GetInt32() / 100m,
                    Currency = disbursement.GetProperty("currency").GetString() ?? "",
                    Status = disbursement.GetProperty("status").GetString() ?? "",
                    Purpose = disbursement.GetProperty("purpose").GetString() ?? "",
                    CreatedAt = disbursement.GetProperty("created_at").GetInt64()
                };
            }

            throw new Exception($"Failed to create disbursement: {responseJson}");
        }

        public async Task<DisbursementResponseDto> GetDisbursementStatusAsync(string disbursementId)
        {
            var response = await _httpClient.GetAsync($"payouts/{disbursementId}");
            var responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var disbursement = JsonSerializer.Deserialize<JsonElement>(responseJson);
                return new DisbursementResponseDto
                {
                    Id = disbursement.GetProperty("id").GetString() ?? "",
                    Entity = disbursement.GetProperty("entity").GetString() ?? "",
                    Amount = disbursement.GetProperty("amount").GetInt32() / 100m,
                    Currency = disbursement.GetProperty("currency").GetString() ?? "",
                    Status = disbursement.GetProperty("status").GetString() ?? "",
                    Purpose = disbursement.GetProperty("purpose").GetString() ?? "",
                    CreatedAt = disbursement.GetProperty("created_at").GetInt64(),
                    FailureReason = disbursement.TryGetProperty("failure_reason", out var reason) ? 
                        reason.GetString() : null
                };
            }

            throw new Exception($"Failed to get disbursement status: {responseJson}");
        }

        private PaymentMethod MapRazorpayMethodToEnum(string method)
        {
            return method.ToLower() switch
            {
                "upi" => PaymentMethod.UPI,
                "netbanking" => PaymentMethod.NetBanking,
                "card" => PaymentMethod.Card,
                _ => PaymentMethod.UPI
            };
        }

        private string ComputeHmacSha256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
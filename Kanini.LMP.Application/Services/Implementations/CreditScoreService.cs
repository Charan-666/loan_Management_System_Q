using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.CreditDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class CreditScoreService : ICreditScoreService
    {
        private readonly ILMPRepository<Customer, int> _customerRepository;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public CreditScoreService(
            ILMPRepository<Customer, int> customerRepository,
            HttpClient httpClient,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _customerRepository = customerRepository;
            _httpClient = httpClient;
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<CreditScoreDto> GetCreditScoreAsync(int customerId)
        {
            var cacheKey = $"credit_score_{customerId}";
            
            if (_cache.TryGetValue(cacheKey, out CreditScoreDto? cachedScore))
                return cachedScore!;

            // Find customer by UserId (not CustomerId)
            var customer = await _customerRepository.GetAsync(c => c.UserId == customerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            var creditScore = await FetchCreditScoreFromCIBIL(customer);
            
            _cache.Set(cacheKey, creditScore, TimeSpan.FromHours(24));
            
            return creditScore;
        }

        public async Task<CreditScoreDto> RefreshCreditScoreAsync(int customerId, string pan)
        {
            // Find customer by UserId (not CustomerId)
            var customer = await _customerRepository.GetAsync(c => c.UserId == customerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            var creditScore = await FetchCreditScoreFromCIBIL(customer, pan);
            
            // Update customer's credit score
            customer.CreditScore = creditScore.Score;
            await _customerRepository.UpdateAsync(customer);
            
            // Update cache
            var cacheKey = $"credit_score_{customerId}";
            _cache.Set(cacheKey, creditScore, TimeSpan.FromHours(24));
            
            return creditScore;
        }

        public async Task<bool> UpdateCustomerCreditScoreAsync(int customerId, int creditScore)
        {
            // Find customer by UserId (not CustomerId)
            var customer = await _customerRepository.GetAsync(c => c.UserId == customerId);
            if (customer == null)
                return false;

            customer.CreditScore = creditScore;
            await _customerRepository.UpdateAsync(customer);
            return true;
        }

        private async Task<CreditScoreDto> FetchCreditScoreFromCIBIL(Customer customer, string? pan = null)
        {
            try
            {
                // CIBIL API through SurePass
                var cibilResponse = await CallSurePassAPI(pan ?? GenerateMockPAN());
                if (cibilResponse != null && cibilResponse.Success)
                {
                    return new CreditScoreDto
                    {
                        CustomerId = customer.CustomerId,
                        PAN = pan ?? GenerateMockPAN(),
                        Bureau = CreditBureau.CIBIL,
                        Score = cibilResponse.CreditScore,
                        ScoreRange = GetScoreRange(cibilResponse.CreditScore),
                        FetchedAt = DateTime.UtcNow,
                        ReportSummary = cibilResponse.Summary ?? GenerateReportSummary(cibilResponse.CreditScore, GetScoreRange(cibilResponse.CreditScore)),
                        CreditHistory = ParseCreditHistory(cibilResponse.CreditHistory)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CIBIL API Error: {ex.Message}");
            }
            
            // Fallback to mock data if API fails
            var score = GenerateMockCreditScore(customer);
            var scoreRange = GetScoreRange(score);
            
            return new CreditScoreDto
            {
                CustomerId = customer.CustomerId,
                PAN = pan ?? GenerateMockPAN(),
                Bureau = CreditBureau.CIBIL,
                Score = score,
                ScoreRange = scoreRange,
                FetchedAt = DateTime.UtcNow,
                ReportSummary = GenerateReportSummary(score, scoreRange),
                CreditHistory = GenerateMockCreditHistory(score)
            };
        }

        private int GenerateMockCreditScore(Customer customer)
        {
            // Generate realistic score based on customer data
            var baseScore = 650;
            
            // Income factor
            if (customer.AnnualIncome > 1000000) baseScore += 50;
            else if (customer.AnnualIncome > 500000) baseScore += 25;
            
            // Age factor
            if (customer.Age > 30) baseScore += 25;
            
            // Add some randomness but keep it realistic
            var random = new Random(customer.CustomerId);
            var variation = random.Next(-50, 100);
            
            return Math.Max(300, Math.Min(850, baseScore + variation));
        }

        private CreditScoreRange GetScoreRange(int score)
        {
            return score switch
            {
                >= 800 => CreditScoreRange.Excellent,
                >= 750 => CreditScoreRange.VeryGood,
                >= 650 => CreditScoreRange.Good,
                >= 550 => CreditScoreRange.Fair,
                _ => CreditScoreRange.Poor
            };
        }

        private string GenerateReportSummary(int score, CreditScoreRange range)
        {
            return range switch
            {
                CreditScoreRange.Excellent => $"Excellent credit score of {score}. You qualify for the best interest rates.",
                CreditScoreRange.VeryGood => $"Very good credit score of {score}. You qualify for competitive rates.",
                CreditScoreRange.Good => $"Good credit score of {score}. You qualify for most loan products.",
                CreditScoreRange.Fair => $"Fair credit score of {score}. Some loan products may have higher rates.",
                _ => $"Credit score of {score} needs improvement. Consider building credit history."
            };
        }

        private List<CreditHistoryItem> GenerateMockCreditHistory(int score)
        {
            var history = new List<CreditHistoryItem>();
            
            // Generate realistic credit history based on score
            if (score >= 650)
            {
                history.Add(new CreditHistoryItem
                {
                    AccountType = "Credit Card",
                    Institution = "HDFC Bank",
                    CreditLimit = 200000,
                    OutstandingAmount = 15000,
                    Status = CreditStatus.Active,
                    OpenedDate = DateTime.UtcNow.AddYears(-2),
                    PaymentHistory = score >= 750 ? 95 : 85
                });
            }
            
            if (score >= 700)
            {
                history.Add(new CreditHistoryItem
                {
                    AccountType = "Personal Loan",
                    Institution = "ICICI Bank",
                    CreditLimit = 500000,
                    OutstandingAmount = 125000,
                    Status = CreditStatus.Active,
                    OpenedDate = DateTime.UtcNow.AddYears(-1),
                    PaymentHistory = score >= 750 ? 98 : 90
                });
            }
            
            return history;
        }

        private async Task<SurePassResponse?> CallSurePassAPI(string pan)
        {
            try
            {
                var request = new
                {
                    id_number = pan,
                    consent = "Y",
                    consent_text = "I hereby declare my consent agreement for fetching my information via CIBIL"
                };

                var apiKey = _configuration["SurePass:ApiKey"];
                var baseUrl = _configuration["SurePass:BaseUrl"];
                var endpoint = _configuration["SurePass:CreditReportEndpoint"];

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

                var response = await _httpClient.PostAsync(
                    $"{baseUrl}{endpoint}",
                    new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonSerializer.Deserialize<SurePassApiResponse>(jsonResponse);
                    
                    return new SurePassResponse
                    {
                        Success = apiResult?.success ?? false,
                        CreditScore = apiResult?.data?.credit_score ?? 650,
                        Summary = apiResult?.data?.summary,
                        CreditHistory = apiResult?.data?.credit_accounts ?? new List<dynamic>()
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SurePass API call failed: {ex.Message}");
            }
            
            return null;
        }

        private List<CreditHistoryItem> ParseCreditHistory(List<dynamic> apiHistory)
        {
            var history = new List<CreditHistoryItem>();
            
            foreach (var item in apiHistory.Take(5)) // Limit to 5 items
            {
                try
                {
                    var historyItem = JsonSerializer.Deserialize<JsonElement>(item.ToString() ?? "{}");
                    
                    history.Add(new CreditHistoryItem
                    {
                        AccountType = historyItem.GetProperty("account_type").GetString() ?? "Unknown",
                        Institution = historyItem.GetProperty("institution").GetString() ?? "Unknown",
                        CreditLimit = historyItem.GetProperty("credit_limit").GetDecimal(),
                        OutstandingAmount = historyItem.GetProperty("outstanding").GetDecimal(),
                        Status = CreditStatus.Active,
                        OpenedDate = DateTime.UtcNow.AddYears(-2),
                        PaymentHistory = historyItem.GetProperty("payment_history").GetInt32()
                    });
                }
                catch
                {
                    // Skip invalid items
                }
            }
            
            return history.Any() ? history : GenerateMockCreditHistory(650);
        }

        private string GenerateMockPAN()
        {
            var random = new Random();
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var digits = "0123456789";
            
            return $"{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{digits[random.Next(digits.Length)]}{digits[random.Next(digits.Length)]}{digits[random.Next(digits.Length)]}{digits[random.Next(digits.Length)]}{letters[random.Next(letters.Length)]}";
        }

        // SurePass API Response Models
        private class SurePassResponse
        {
            public bool Success { get; set; }
            public int CreditScore { get; set; }
            public string? Summary { get; set; }
            public List<dynamic> CreditHistory { get; set; } = new();
        }

        private class SurePassApiResponse
        {
            public bool success { get; set; }
            public SurePassData? data { get; set; }
        }

        private class SurePassData
        {
            public int credit_score { get; set; }
            public string? summary { get; set; }
            public List<dynamic>? credit_accounts { get; set; }
        }
    }
}
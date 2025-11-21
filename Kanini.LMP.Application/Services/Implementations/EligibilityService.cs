using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.CreditDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EligibilityService : IEligibilityService
    {
        private readonly ILMPRepository<Customer, int> _customerRepository;

        public EligibilityService(ILMPRepository<Customer, int> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<EligibilityScoreDto> CalculateEligibilityAsync(int customerId, int loanProductId)
        {
            // Find customer by UserId (not CustomerId)
            var customer = await _customerRepository.GetAsync(c => c.UserId == customerId);
            if (customer == null) throw new ArgumentException("Customer not found");

            // Use stored credit score (mock or updated from profile)
            var creditScore = customer.CreditScore;

            var monthlyIncome = customer.AnnualIncome / 12;
            var eligibilityScore = CalculateScore(customer, loanProductId, (int)creditScore);
            var status = DetermineStatus(eligibilityScore, loanProductId);

            return new EligibilityScoreDto
            {
                CustomerId = customer.CustomerId,
                LoanProductId = loanProductId,
                CreditScore = (int)creditScore,
                MonthlyIncome = monthlyIncome,
                ExistingEMIAmount = 0, // Default - can be enhanced
                DebtToIncomeRatio = 0, // Default - can be enhanced
                EmploymentType = customer.Occupation,
                EligibilityScore = eligibilityScore,
                EligibilityStatus = status,
                CalculatedOn = DateTime.UtcNow
            };
        }

        public async Task<bool> IsEligibleForLoanAsync(int customerId, int loanProductId = 0)
        {
            var eligibility = await CalculateEligibilityAsync(customerId, loanProductId);
            
            // Home Loan (ID 3) requires higher score
            if (loanProductId == 3) return eligibility.EligibilityScore >= 65;
            
            // Personal & Vehicle Loans require lower score
            return eligibility.EligibilityScore >= 55;
        }

        public async Task<List<int>> GetEligibleProductsAsync(int customerId)
        {
            var eligibility = await CalculateEligibilityAsync(customerId, 0);
            var eligibleProducts = new List<int>();
            
            if (eligibility.EligibilityScore >= 55)
            {
                eligibleProducts.Add(1); // Personal Loan
                eligibleProducts.Add(2); // Vehicle Loan
            }
            
            if (eligibility.EligibilityScore >= 65)
            {
                eligibleProducts.Add(3); // Home Loan
            }
            
            return eligibleProducts;
        }

        public async Task UpdateCustomerProfileAsync(int userId, EligibilityProfileRequest request)
        {
            var customer = await _customerRepository.GetAsync(c => c.UserId == userId);
            if (customer == null) throw new ArgumentException("Customer not found");

            // For new users, update basic profile info
            if (!request.IsExistingBorrower)
            {
                customer.AnnualIncome = request.AnnualIncome ?? customer.AnnualIncome;
                customer.Occupation = request.Occupation ?? customer.Occupation;
                customer.HomeOwnershipStatus = request.HomeOwnershipStatus ?? customer.HomeOwnershipStatus;
            }
            // For existing users, only update if new values provided
            else
            {
                if (request.AnnualIncome.HasValue) customer.AnnualIncome = request.AnnualIncome.Value;
                if (!string.IsNullOrEmpty(request.Occupation)) customer.Occupation = request.Occupation;
                if (request.HomeOwnershipStatus.HasValue) customer.HomeOwnershipStatus = request.HomeOwnershipStatus.Value;
            }

            customer.UpdatedAt = DateTime.UtcNow;

            // Create merged request with existing customer data for score calculation
            var mergedRequest = CreateMergedRequest(customer, request);
            customer.CreditScore = CalculateMockCreditScore(mergedRequest);

            await _customerRepository.UpdateAsync(customer);
        }

        private EligibilityProfileRequest CreateMergedRequest(Customer customer, EligibilityProfileRequest request)
        {
            return new EligibilityProfileRequest
            {
                IsExistingBorrower = request.IsExistingBorrower,
                AnnualIncome = request.AnnualIncome ?? customer.AnnualIncome,
                Occupation = request.Occupation ?? customer.Occupation,
                HomeOwnershipStatus = request.HomeOwnershipStatus ?? customer.HomeOwnershipStatus,
                Age = request.Age ?? customer.Age,
                // Additional fields from request (only for existing borrowers)
                ExperienceYears = request.ExperienceYears,
                EmployerName = request.EmployerName,
                MonthlyEMI = request.MonthlyEMI,
                ExistingLoanAmount = request.ExistingLoanAmount,
                PreviousLoanCount = request.PreviousLoanCount,
                OnTimePayments = request.OnTimePayments,
                LatePayments = request.LatePayments,
                MissedPayments = request.MissedPayments,
                HasDefaultHistory = request.HasDefaultHistory,
                DaysOverdueMax = request.DaysOverdueMax
            };
        }

        private int CalculateMockCreditScore(EligibilityProfileRequest request)
        {
            if (request.IsExistingBorrower)
            {
                return CalculateExistingBorrowerScore(request);
            }
            else
            {
                return CalculateNewBorrowerScore(request);
            }
        }

        private int CalculateNewBorrowerScore(EligibilityProfileRequest request)
        {
            int score = 650; // Higher base for new borrowers (no negative history)

            // Income factor (50% weightage)
            if (request.AnnualIncome >= 1000000) score += 100;
            else if (request.AnnualIncome >= 500000) score += 80;
            else if (request.AnnualIncome >= 300000) score += 60;
            else if (request.AnnualIncome >= 200000) score += 40;

            // Occupation factor (30% weightage)
            var occupation = request.Occupation.ToLower();
            if (occupation.Contains("government") || occupation.Contains("bank")) score += 70;
            else if (occupation.Contains("engineer") || occupation.Contains("doctor") || occupation.Contains("manager")) score += 60;
            else if (occupation.Contains("teacher") || occupation.Contains("consultant")) score += 50;
            else if (occupation.Contains("business") || occupation.Contains("self")) score += 30;
            else score += 20;

            // Home ownership (20% weightage)
            if (request.HomeOwnershipStatus == HomeOwnershipStatus.Owned) score += 50;
            else if (request.HomeOwnershipStatus == HomeOwnershipStatus.Mortage) score += 30;
            else score += 10; // Rented

            return Math.Min(900, Math.Max(300, score)); // Cap between 300-900
        }

        private int CalculateExistingBorrowerScore(EligibilityProfileRequest request)
        {
            int score = 600; // Lower base for existing borrowers (risk assessment)

            // Income factor (35% weightage)
            if (request.AnnualIncome >= 1000000) score += 80;
            else if (request.AnnualIncome >= 500000) score += 65;
            else if (request.AnnualIncome >= 300000) score += 50;
            else if (request.AnnualIncome >= 200000) score += 35;

            // Occupation & Experience (25% weightage)
            var occupation = request.Occupation.ToLower();
            int occupationScore = 0;
            if (occupation.Contains("government") || occupation.Contains("bank")) occupationScore = 50;
            else if (occupation.Contains("engineer") || occupation.Contains("doctor")) occupationScore = 45;
            else if (occupation.Contains("manager") || occupation.Contains("consultant")) occupationScore = 40;
            else if (occupation.Contains("business")) occupationScore = 25;
            else occupationScore = 15;

            // Experience bonus
            if (request.ExperienceYears >= 10) occupationScore += 15;
            else if (request.ExperienceYears >= 5) occupationScore += 10;
            else if (request.ExperienceYears >= 2) occupationScore += 5;

            score += occupationScore;

            // Debt-to-Income Ratio (25% weightage)
            var monthlyIncome = request.AnnualIncome / 12;
            var currentEMI = request.MonthlyEMI ?? 0;
            var debtRatio = monthlyIncome > 0 ? (currentEMI / monthlyIncome) : 0;
            
            if (debtRatio <= 0.2m) score += 50; // Excellent debt ratio
            else if (debtRatio <= 0.4m) score += 30; // Good debt ratio
            else if (debtRatio <= 0.6m) score += 10; // Acceptable
            else score -= 20; // High debt burden

            // Credit History & Payment Behavior (15% weightage)
            score += CalculatePaymentBehaviorScore(request);

            // Home ownership (10% weightage - less important for existing borrowers)
            if (request.HomeOwnershipStatus == HomeOwnershipStatus.Owned) score += 25;
            else if (request.HomeOwnershipStatus == HomeOwnershipStatus.Mortage) score += 15;
            else score += 5;

            return Math.Min(900, Math.Max(300, score)); // Cap between 300-900
        }

        private int CalculatePaymentBehaviorScore(EligibilityProfileRequest request)
        {
            int paymentScore = 0;
            
            // If no payment history provided, give neutral score
            if (request.OnTimePayments == null && request.LatePayments == null && request.MissedPayments == null)
            {
                return request.PreviousLoanCount >= 1 ? 15 : 5; // Basic score for loan count
            }

            var totalPayments = (request.OnTimePayments ?? 0) + (request.LatePayments ?? 0) + (request.MissedPayments ?? 0);
            
            if (totalPayments == 0) return 5; // No payment data

            // Calculate payment ratios
            var onTimeRatio = (decimal)(request.OnTimePayments ?? 0) / totalPayments;
            var lateRatio = (decimal)(request.LatePayments ?? 0) / totalPayments;
            var missedRatio = (decimal)(request.MissedPayments ?? 0) / totalPayments;

            // On-time payment scoring (0-50 points)
            if (onTimeRatio >= 0.95m) paymentScore += 50; // Excellent: 95%+ on time
            else if (onTimeRatio >= 0.90m) paymentScore += 40; // Very Good: 90-94% on time
            else if (onTimeRatio >= 0.80m) paymentScore += 30; // Good: 80-89% on time
            else if (onTimeRatio >= 0.70m) paymentScore += 20; // Fair: 70-79% on time
            else if (onTimeRatio >= 0.50m) paymentScore += 10; // Poor: 50-69% on time
            else paymentScore -= 10; // Very Poor: <50% on time

            // Late payment penalty (0 to -20 points)
            if (lateRatio > 0.3m) paymentScore -= 20; // >30% late payments
            else if (lateRatio > 0.2m) paymentScore -= 15; // 20-30% late
            else if (lateRatio > 0.1m) paymentScore -= 10; // 10-20% late
            else if (lateRatio > 0.05m) paymentScore -= 5; // 5-10% late

            // Missed payment penalty (0 to -30 points)
            if (missedRatio > 0.1m) paymentScore -= 30; // >10% missed
            else if (missedRatio > 0.05m) paymentScore -= 20; // 5-10% missed
            else if (missedRatio > 0.02m) paymentScore -= 10; // 2-5% missed
            else if (missedRatio > 0) paymentScore -= 5; // Any missed payments

            // Default history penalty
            if (request.HasDefaultHistory == true) paymentScore -= 40;

            // Severity of overdue penalty
            if (request.DaysOverdueMax > 90) paymentScore -= 25; // 90+ days overdue
            else if (request.DaysOverdueMax > 60) paymentScore -= 15; // 60-90 days
            else if (request.DaysOverdueMax > 30) paymentScore -= 10; // 30-60 days
            else if (request.DaysOverdueMax > 0) paymentScore -= 5; // 1-30 days

            // Experience bonus for consistent good behavior
            if (onTimeRatio >= 0.95m && totalPayments >= 24) paymentScore += 10; // 2+ years excellent history

            return Math.Max(-50, Math.Min(50, paymentScore)); // Cap between -50 to +50
        }

        private double CalculateScore(Customer customer, int loanProductId, int realTimeCreditScore)
        {
            double score = 0;

            // Credit Score (40% weightage) - Using real-time score
            if (realTimeCreditScore >= 750) score += 40;
            else if (realTimeCreditScore >= 700) score += 30;
            else if (realTimeCreditScore >= 650) score += 20;
            else if (realTimeCreditScore >= 600) score += 10;

            // Income (30% weightage)
            var monthlyIncome = customer.AnnualIncome / 12;
            if (monthlyIncome >= 100000) score += 30;
            else if (monthlyIncome >= 50000) score += 25;
            else if (monthlyIncome >= 30000) score += 20;
            else if (monthlyIncome >= 20000) score += 15;

            // Age (15% weightage)
            if (customer.Age >= 25 && customer.Age <= 55) score += 15;
            else if (customer.Age >= 21 && customer.Age <= 60) score += 10;

            // Home Ownership (10% weightage)
            if (customer.HomeOwnershipStatus == HomeOwnershipStatus.Owned) score += 10;
            else if (customer.HomeOwnershipStatus == HomeOwnershipStatus.Mortage) score += 5;

            // Occupation (5% weightage)
            if (customer.Occupation.Contains("Engineer") || customer.Occupation.Contains("Manager")) score += 5;

            return Math.Round(score, 2);
        }

        private string DetermineStatus(double score, int loanProductId)
        {
            if (score >= 65) return "Eligible for All Products";
            if (score >= 55) return "Eligible for Personal & Vehicle Loans";
            return "Not Eligible";
        }
    }
}
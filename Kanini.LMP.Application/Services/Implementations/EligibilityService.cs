using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
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
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null) throw new ArgumentException("Customer not found");

            var monthlyIncome = customer.AnnualIncome / 12;
            var eligibilityScore = CalculateScore(customer, loanProductId);
            var status = DetermineStatus(eligibilityScore, loanProductId);

            return new EligibilityScoreDto
            {
                CustomerId = customerId,
                LoanProductId = loanProductId,
                CreditScore = (int)customer.CreditScore,
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

        private double CalculateScore(Customer customer, int loanProductId)
        {
            double score = 0;

            // Credit Score (40% weightage)
            if (customer.CreditScore >= 750) score += 40;
            else if (customer.CreditScore >= 700) score += 30;
            else if (customer.CreditScore >= 650) score += 20;
            else if (customer.CreditScore >= 600) score += 10;

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
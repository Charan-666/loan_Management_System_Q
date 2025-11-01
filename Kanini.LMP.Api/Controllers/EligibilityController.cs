using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EligibilityController : ControllerBase
    {
        private readonly IEligibilityService _eligibilityService;

        public EligibilityController(IEligibilityService eligibilityService)
        {
            _eligibilityService = eligibilityService;
        }

        [HttpGet("check-product/{customerId}/{loanProductId}")]
        public async Task<ActionResult<EligibilityScoreDto>> CheckEligibility(int customerId, int loanProductId)
        {
            var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, loanProductId);
            return Ok(eligibility);
        }

        [HttpGet("product-status/{customerId}/{loanProductId}")]
        public async Task<ActionResult<bool>> GetEligibilityStatus(int customerId, int loanProductId)
        {
            var isEligible = await _eligibilityService.IsEligibleForLoanAsync(customerId, loanProductId);
            return Ok(new { isEligible, message = isEligible ? "Customer is eligible" : "Customer is not eligible" });
        }

        [HttpGet("overall/{customerId}")]
        public async Task<ActionResult> CheckOverallEligibility(int customerId)
        {
            var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, 0);
            var eligibleProductIds = await _eligibilityService.GetEligibleProductsAsync(customerId);
            
            var allProducts = new[]
            {
                new { ProductId = 1, ProductName = "Personal Loan", Available = eligibleProductIds.Contains(1) },
                new { ProductId = 2, ProductName = "Vehicle Loan", Available = eligibleProductIds.Contains(2) },
                new { ProductId = 3, ProductName = "Home Loan", Available = eligibleProductIds.Contains(3) }
            };

            var message = eligibility.EligibilityScore switch
            {
                >= 65 => "Congratulations! You can apply for all loan products.",
                >= 55 => "You can apply for Personal and Vehicle loans. Score 65+ needed for Home Loan.",
                _ => $"Score {eligibility.EligibilityScore}/100. Need 55+ to apply for loans."
            };

            return Ok(new
            {
                CustomerId = customerId,
                EligibilityScore = eligibility.EligibilityScore,
                Status = eligibility.EligibilityStatus,
                EligibleProductCount = eligibleProductIds.Count,
                Message = message,
                Products = allProducts
            });
        }
    }
}
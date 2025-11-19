using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.CreditDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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



        [HttpPost("check")]
        public async Task<ActionResult> CheckEligibility([FromBody] CreditScoreRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                
                // Calculate eligibility with PAN for fresh credit score
                var eligibility = await _eligibilityService.CalculateEligibilityAsync(userId, 0, request.PAN);
                var eligibleProductIds = await _eligibilityService.GetEligibleProductsAsync(userId);
                
                var allProducts = new[]
                {
                    new { ProductId = 1, ProductName = "Personal Loan", Available = eligibleProductIds.Contains(1), MinScore = 55 },
                    new { ProductId = 2, ProductName = "Vehicle Loan", Available = eligibleProductIds.Contains(2), MinScore = 55 },
                    new { ProductId = 3, ProductName = "Home Loan", Available = eligibleProductIds.Contains(3), MinScore = 65 }
                };

                var message = eligibility.EligibilityScore switch
                {
                    >= 65 => "Congratulations! You can apply for all loan products.",
                    >= 55 => "You can apply for Personal and Vehicle loans. Score 65+ needed for Home Loan.",
                    _ => $"Score {eligibility.EligibilityScore}/100. Need 55+ to apply for loans."
                };

                return Ok(new
                {
                    CustomerId = eligibility.CustomerId,
                    CreditScore = eligibility.CreditScore,
                    EligibilityScore = eligibility.EligibilityScore,
                    Status = eligibility.EligibilityStatus,
                    EligibleProductCount = eligibleProductIds.Count,
                    Message = message,
                    Products = allProducts,
                    LastUpdated = eligibility.CalculatedOn
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
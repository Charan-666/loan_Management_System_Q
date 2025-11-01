using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.KYC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KYCController : ControllerBase
    {
        private readonly IKYCService _kycService;

        public KYCController(IKYCService kycService)
        {
            _kycService = kycService;
        }

        // Customer endpoints
        [HttpPost("submit-document")]
        public async Task<ActionResult> SubmitKYCDocument([FromBody] KYCSubmissionDto kycDto)
        {
            var result = await _kycService.SubmitKYCDocumentAsync(kycDto);
            if (result)
                return Ok(new { success = true, message = "KYC document submitted successfully" });
            
            return BadRequest(new { success = false, message = "Failed to submit KYC document" });
        }

        [HttpGet("status/{customerId}")]
        public async Task<ActionResult<KYCStatusDto>> GetKYCStatus(int customerId)
        {
            var status = await _kycService.GetCustomerKYCStatusAsync(customerId);
            return Ok(status);
        }

        [HttpGet("my-status")]
        public async Task<ActionResult<KYCStatusDto>> GetMyKYCStatus()
        {
            var userId = GetCurrentUserId();
            var status = await _kycService.GetCustomerKYCStatusAsync(userId);
            return Ok(status);
        }

        [HttpGet("is-completed/{customerId}")]
        public async Task<ActionResult<bool>> IsKYCCompleted(int customerId)
        {
            var isCompleted = await _kycService.IsKYCCompletedAsync(customerId);
            return Ok(new { isCompleted });
        }

        // Manager endpoints
        [HttpGet("pending-documents")]
        public async Task<ActionResult<IEnumerable<KYCVerificationDto>>> GetPendingKYCDocuments()
        {
            var documents = await _kycService.GetPendingKYCDocumentsAsync();
            return Ok(documents);
        }

        [HttpGet("document/{documentId}")]
        public async Task<ActionResult<KYCVerificationDto>> GetKYCDocumentDetails(int documentId)
        {
            try
            {
                var document = await _kycService.GetKYCDocumentDetailsAsync(documentId);
                return Ok(document);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("verify")]
        public async Task<ActionResult> VerifyKYCDocument([FromBody] KYCVerificationRequestDto verificationDto)
        {
            var result = await _kycService.VerifyKYCDocumentAsync(verificationDto);
            if (result)
                return Ok(new { success = true, message = "KYC document verified successfully" });
            
            return BadRequest(new { success = false, message = "Failed to verify KYC document" });
        }

        [HttpPost("reject")]
        public async Task<ActionResult> RejectKYCDocument([FromBody] KYCVerificationRequestDto rejectionDto)
        {
            var result = await _kycService.RejectKYCDocumentAsync(rejectionDto);
            if (result)
                return Ok(new { success = true, message = "KYC document rejected" });
            
            return BadRequest(new { success = false, message = "Failed to reject KYC document" });
        }

        [HttpGet("score/{customerId}")]
        public async Task<ActionResult<decimal>> GetKYCScore(int customerId)
        {
            var score = await _kycService.CalculateKYCScoreAsync(customerId);
            return Ok(new { score });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}
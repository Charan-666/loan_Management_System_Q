using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RazorpayController : ControllerBase
    {
        private readonly IRazorpayService _razorpayService;

        public RazorpayController(IRazorpayService razorpayService)
        {
            _razorpayService = razorpayService;
        }

        [HttpPost("create-order")]
        public async Task<ActionResult<RazorpayOrderResponseDto>> CreateOrder([FromBody] RazorpayOrderCreateDto orderDto)
        {
            try
            {
                var order = await _razorpayService.CreateOrderAsync(orderDto);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("process-payment")]
        public async Task<ActionResult<PaymentTransactionResponseDTO>> ProcessPayment([FromBody] RazorpayPaymentDto paymentDto)
        {
            try
            {
                var result = await _razorpayService.ProcessPaymentAsync(paymentDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-signature")]
        public async Task<ActionResult<bool>> VerifySignature([FromBody] RazorpayPaymentDto paymentDto)
        {
            try
            {
                var isValid = await _razorpayService.VerifyPaymentSignatureAsync(
                    paymentDto.RazorpayOrderId, 
                    paymentDto.RazorpayPaymentId, 
                    paymentDto.RazorpaySignature);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("disburse-loan")]
        public async Task<ActionResult<DisbursementResponseDto>> DisburseLoan([FromBody] DisbursementDto disbursementDto)
        {
            try
            {
                var disbursement = await _razorpayService.CreateDisbursementAsync(disbursementDto);
                return Ok(disbursement);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("disbursement-status/{disbursementId}")]
        public async Task<ActionResult<DisbursementResponseDto>> GetDisbursementStatus(string disbursementId)
        {
            try
            {
                var status = await _razorpayService.GetDisbursementStatusAsync(disbursementId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
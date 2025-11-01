using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using EntityPaymentStatus = Kanini.LMP.Database.Entities.PaymentStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("loan-account/{loanAccountId}")]
        public async Task<ActionResult<IReadOnlyList<PaymentTransactionDTO>>> GetPaymentsByLoanAccount(int loanAccountId)
        {
            var payments = await _paymentService.GetPaymentsByLoanAccountAsync(loanAccountId);
            return Ok(payments);
        }

        [HttpGet("emi-plans/{loanAccountId}")]
        public async Task<ActionResult> GetEMIPlans(int loanAccountId)
        {
            var emiPlans = await _paymentService.GetEMIPlansByLoanAccountAsync(loanAccountId);
            return Ok(emiPlans);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentTransactionDTO>> CreatePayment(PaymentTransactionCreateDTO dto)
        {
            var created = await _paymentService.CreatePaymentAsync(dto);
            return CreatedAtAction(nameof(GetPaymentsByLoanAccount), new { loanAccountId = created.LoanAccountId }, created);
        }

        [HttpPut("{paymentId}/status")]
        public async Task<ActionResult<PaymentTransactionDTO>> UpdatePaymentStatus(int paymentId, [FromBody] EntityPaymentStatus status)
        {
            var updated = await _paymentService.UpdatePaymentStatusAsync(paymentId, status);
            return Ok(updated);
        }
    }
}
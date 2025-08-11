using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Services.Interfaces;

namespace TransactionManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaystackService _paymentService;
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaystackService paymentServices,
                                  IMediator mediator,
                                  ILogger<PaymentsController> logger)
        {
            _paymentService = paymentServices;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("initializePayment")]
        public async Task<ActionResult<PaymentInitializationResponse>> InitializePayment([FromBody] PaymentRequest request)
        {
            var response = await _paymentService.InitializePaymentAsync(request.Amount,request.Email,request.Reference);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("verifyPayment/{reference}")]
        public async Task<ActionResult<PaymentVerificationResponse>> VerifyPayment(string reference)
        {
            var response = await _paymentService.VerifyPaymentAsync(reference);

            if (response.Success && response.Message == "success")
            {
                // Process the payment as a deposit
                // Note: You'll need to map the reference to an account
                // This is simplified - in reality you'd store this mapping during initialization

                _logger.LogInformation($"Payment verified successfully: {reference}");
            }

            return Ok(response);
        }

        [HttpPost("processWithdraw")]
        public async Task<ActionResult<WithdrawalResponse>> ProcessWithdrawal([FromBody] WithdrawalRequest request)
        {
            var response = await _paymentService.ProcessWithdrawalAsync(request);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
    }
}

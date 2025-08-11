using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(IMediator mediator, ILogger<TransactionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("deposit")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Deposit([FromBody] DepositCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing deposit");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transfer");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}

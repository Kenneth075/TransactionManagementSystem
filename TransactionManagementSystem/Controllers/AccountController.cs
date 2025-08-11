using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Application.Query;

namespace TransactionManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMediator mediator, ILogger<AccountController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
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
                _logger.LogError(ex, "Error creating account");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{accountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAccountDetails(Guid accountId)
        {
            try
            {
                var query = new GetAccountDetailsQuery { AccountId = accountId };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account details for {AccountId}", accountId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{accountId}/transactions")]
        public async Task<IActionResult> GetTransactionHistory(Guid accountId,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10,[FromQuery] DateTime? fromDate = null,[FromQuery] DateTime? toDate = null)
        {
            try
            {
                var query = new GetTransactionHistoryQuery
                {
                    AccountId = accountId,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction history for {AccountId}", accountId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}


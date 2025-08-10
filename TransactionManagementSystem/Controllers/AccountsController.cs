using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Query;

namespace TransactionManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount()
        {

            var userId = GetCurrentUserId();
            var command = new CreateAccountCommand { UserId = userId };
            var accountId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetAccount), new { id = accountId }, accountId);

        }

        [HttpGet("GetAccount/{id}")]
        public async Task<ActionResult<AccountDto>> GetAccount(Guid id)
        {
            var query = new GetAccountByIdQuery { AccountId = id };
            var account = await _mediator.Send(query);

            if (account == null)
                return NotFound();

            // Ensure user can only access their own accounts
            if (account.User.Id != GetCurrentUserId() && !IsAdmin())
                return Forbid();

            return Ok(account);
        }

        [HttpGet("GetUserAccounts/{userId}")]
        public async Task<ActionResult<List<AccountDto>>> GetUserAccounts(Guid userId)
        {
            // Users can only access their own accounts unless they are admin
            if (userId != GetCurrentUserId() && !IsAdmin())
                return Forbid();

            var query = new GetAccountByUserIdQuery { UserId = userId };
            var accounts = await _mediator.Send(query);

            return Ok(accounts);
        }

        [HttpGet("{id}/GetAccountBalance")]
        public async Task<ActionResult<decimal>> GetAccountBalance(Guid id)
        {
            var query = new GetAccountBalanceQuery { AccountId = id };
            var balance = await _mediator.Send(query);

            return Ok(new { AccountId = id, Balance = balance });
        }

        [HttpGet("{id}/GetTransactionHistory")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactionHistory(
            Guid id,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = new GetTransactionHistoryQuery
            {
                AccountId = id,
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var transactions = await _mediator.Send(query);
            return Ok(transactions);
        }

        [HttpGet("{id}/GetMonthlyStatement/{year}/{month}")]
        public async Task<ActionResult<MonthlyStatementDto>> GetMonthlyStatement(Guid id, int year, int month)
        {
            var query = new GetMonthlyStatementQuery { AccountId = id, Year = year, Month = month };
            var statement = await _mediator.Send(query);

            return Ok(statement);
        }


        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin") || User.IsInRole("Manager");
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("deposit")]
        public async Task<ActionResult<Guid>> Deposit([FromBody] DepositCommand command)
        {
            //var command = new DepositCommand
            //{
            //    AccountId = request.AccountId,
            //    Amount = request.Amount,
            //    Description = request.Description,
            //    Reference = request.Reference
            //};

            var transactionId = await _mediator.Send(command);
            return Ok(new { TransactionId = transactionId });
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<Guid>> Withdraw([FromBody] WithdrawCommand command)
        {
            //var command = new WithdrawCommand
            //{
            //    FromAccountId = request.AccountId,
            //    Amount = request.Amount,
            //    Description = request.Description,
            //    Reference = request.Reference
            //};

            var transactionId = await _mediator.Send(command);
            return Ok(new { TransactionId = transactionId });
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<Guid>> Transfer([FromBody] TransferCommand command)
        {
            //var command = new TransferCommand
            //{
            //    FromAccountId = request.FromAccountId,
            //    ToAccountId = request.ToAccountId,
            //    Amount = request.Amount,
            //    Description = request.Description,
            //    Reference = request.Reference
            //};

            var transactionId = await _mediator.Send(command);
            return Ok(new { TransactionId = transactionId });
        }
    }
}

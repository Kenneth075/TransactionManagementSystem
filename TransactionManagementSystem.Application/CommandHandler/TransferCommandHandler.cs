using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Domain.Exceptions;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.CommandHandler
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Guid>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<TransferCommandHandler> _logger;

        public TransferCommandHandler(AppDbContext appDbContext,
                                      ILogger<TransferCommandHandler> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        public async Task<Guid> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Inside {nameof(TransferCommandHandler)}");

            using var transaction = await _appDbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var fromAccount = await _appDbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.FromAccountId, cancellationToken);
                var toAccount = await _appDbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.ToAccountId, cancellationToken);
                if (fromAccount == null)
                {
                    _logger.LogError($"Source account {request.FromAccountId} not found.");
                    throw new AccountNotFoundException($"Source account {request.FromAccountId} not found.");
                }
                if (toAccount == null)
                {
                    _logger.LogError($"Destination account {request.ToAccountId} not found.");
                    throw new AccountNotFoundException($"Destination account {request.ToAccountId} not found.");
                }

                if (!fromAccount.CanWithdraw(request.Amount))
                {
                    _logger.LogError($"Insufficient balance in account {request.FromAccountId}.");
                    throw new InsufficientFundsException($"Insufficient funds in account {request.FromAccountId}");
                }

                fromAccount.Withdraw(request.Amount);
                toAccount.Deposit(request.Amount);

                var trenasferTransaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    FromAccountId = request.FromAccountId,
                    ToAccountId = request.ToAccountId,
                    Amount = request.Amount,
                    Description = request.Description ?? "Transfer",
                    Reference = request.Reference ?? Guid.NewGuid().ToString(),
                    Type = TransactionType.Transfer,
                    Status = TransactionStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow
                };

                _appDbContext.Transactions.Add(trenasferTransaction);
                var result = await _appDbContext.SaveChangesAsync(cancellationToken);
                if (result <= 0)
                {
                    _logger.LogError("Failed to complete transfer transaction.");
                    throw new Exception("Failed to complete transfer transaction.");
                }
                _logger.LogInformation($"Transfer of {request.Amount:C} completed from {fromAccount} to {toAccount}.");
                await transaction.CommitAsync(cancellationToken);
                return trenasferTransaction.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

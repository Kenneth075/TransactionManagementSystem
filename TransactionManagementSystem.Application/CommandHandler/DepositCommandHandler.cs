using MediatR;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Domain.Exceptions;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.CommandHandler
{
    public class DepositCommandHandler : IRequestHandler<DepositCommand, Guid>
    {
        private readonly AppDbContext _appDBContext;
        private readonly ILogger<DepositCommandHandler> _logger;

        public DepositCommandHandler(AppDbContext appDBContext,
                                     ILogger<DepositCommandHandler> logger)
        {
            _appDBContext = appDBContext;
            _logger = logger;
        }

        public async Task<Guid> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Inside {nameof(DepositCommandHandler)}");
            using var transactions = await _appDBContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var account = await _appDBContext.Accounts.FindAsync(request.AccountId);
                if (account == null)
                {
                    _logger.LogError($"Account with ID {request.AccountId} not found.");
                    throw new AccountNotFoundException($"Account with ID {request.AccountId} not found.");
                }

                account.Deposit(request.Amount);

                var transactionRecord = new Transaction
                {
                    Id = Guid.NewGuid(),
                    FromAccountId = request.AccountId,
                    ToAccountId = null,
                    Amount = request.Amount,
                    Description = request.Description ?? "Deposit",
                    Reference = request.Reference ?? Guid.NewGuid().ToString(),
                    Type = TransactionType.Deposit,
                    Status = TransactionStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow
                };

                _appDBContext.Transactions.Add(transactionRecord);
                var result = await _appDBContext.SaveChangesAsync(cancellationToken);
                if (result <= 0)
                {
                    _logger.LogError("Failed to deposit amount.");
                    throw new Exception("Failed to deposit amount.");
                }
                _logger.LogInformation($"Deposit of {request.Amount:C} to account {request.AccountId} successful.");
                await transactions.CommitAsync(cancellationToken);
                return transactionRecord.Id;
            }
            catch (Exception)
            {
                await transactions.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

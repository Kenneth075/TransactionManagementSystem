using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.CommandHandler
{
    public class WithdrawHandler : IRequestHandler<WithdrawCommand, TransactionResponse>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WithdrawHandler> _logger;

        public WithdrawHandler(AppDbContext context, ILogger<WithdrawHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TransactionResponse> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            //using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
       

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId && a.Status == AccountStatus.Active, cancellationToken);

                if (account == null)
                {
                    return new TransactionResponse
                    {
                        Success = false,
                        Message = "Account not found or inactive"
                    };
                }

                if (request.Amount <= 0)
                {
                    return new TransactionResponse
                    {
                        Success = false,
                        Message = "Invalid amount"
                    };
                }

                if (account.Balance < request.Amount)
                {
                    return new TransactionResponse
                    {
                        Success = false,
                        Message = "Insufficient funds"
                    };
                }

                var withdrawTransaction = new Transaction
                {
                    FromAccountId = request.AccountId,
                    Amount = request.Amount,
                    Type = TransactionType.Withdrawal,
                    Status = TransactionStatus.Completed,
                    Description = request.Description ?? "Withdrawal",
                    ReferenceNumber = GenerateReferenceNumber(),
                    ExternalReference = request.ExternalReference,
                    ProcessedAt = DateTime.UtcNow
                };

                account.Balance -= request.Amount;
                account.UpdatedAt = DateTime.UtcNow;

                _context.Transactions.Add(withdrawTransaction);
                await _context.SaveChangesAsync(cancellationToken);
                //await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Withdrawal successful. AccountId: {AccountId}, Amount: {Amount}, New Balance: {Balance}",
                    request.AccountId, request.Amount, account.Balance);

                return new TransactionResponse
                {
                    TransactionId = withdrawTransaction.Id,
                    ReferenceNumber = withdrawTransaction.ReferenceNumber,
                    Success = true,
                    Message = "Withdrawal successful",
                    NewBalance = account.Balance
                };
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error processing withdrawal for account {AccountId}", request.AccountId);

                return new TransactionResponse
                {
                    Success = false,
                    Message = "Withdrawal failed"
                };
            }
        }

        private string GenerateReferenceNumber()
        {
            return $"WTH{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
    }
}


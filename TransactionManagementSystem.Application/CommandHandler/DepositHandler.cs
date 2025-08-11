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
    public class DepositHandler : IRequestHandler<DepositCommand, TransactionResponse>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DepositHandler> _logger;

        public DepositHandler(AppDbContext context, ILogger<DepositHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TransactionResponse> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

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

                var depositTransaction = new Transaction
                {
                    FromAccountId = request.AccountId,
                    Amount = request.Amount,
                    Type = TransactionType.Deposit,
                    Status = TransactionStatus.Completed,
                    Description = request.Description ?? "Deposit",
                    ReferenceNumber = GenerateReferenceNumber(),
                    ExternalReference = request.ExternalReference,
                    ProcessedAt = DateTime.UtcNow
                };

                account.Balance += request.Amount;
                account.UpdatedAt = DateTime.UtcNow;

                _context.Transactions.Add(depositTransaction);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Deposit successful. AccountId: {AccountId}, Amount: {Amount}, New Balance: {Balance}",
                    request.AccountId, request.Amount, account.Balance);

                return new TransactionResponse
                {
                    TransactionId = depositTransaction.Id,
                    ReferenceNumber = depositTransaction.ReferenceNumber,
                    Success = true,
                    Message = "Deposit successful",
                    NewBalance = account.Balance
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error processing deposit for account {AccountId}", request.AccountId);

                return new TransactionResponse
                {
                    Success = false,
                    Message = "Deposit failed"
                };
            }
        }

        private string GenerateReferenceNumber()
        {
            return $"DEP{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
    }
}


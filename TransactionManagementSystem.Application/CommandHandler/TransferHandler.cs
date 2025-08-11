using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.CommandHandler
{
    public class TransferHandler : IRequestHandler<TransferCommand, TransactionResponse>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TransferHandler> _logger;

        public TransferHandler(AppDbContext context, ILogger<TransferHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TransactionResponse> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Validate both accounts exist and are active
                var fromAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.FromAccountId && a.Status == AccountStatus.Active, cancellationToken);

                var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.ToAccountId && a.Status == AccountStatus.Active, cancellationToken);

                if (fromAccount == null)
                {
                    return new TransactionResponse
                    {
                        Success = false,
                        Message = "Source account not found or inactive"
                    };
                }

                if (toAccount == null)
                {
                    return new TransactionResponse
                    {
                        Success = false,
                        Message = "Destination account not found or inactive"
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

                if (fromAccount.Balance < request.Amount)
                {
                    return new TransactionResponse
                    {
                        Success = false,
                        Message = "Insufficient funds"
                    };
                }

                // Prevent self-transfer
                if (request.FromAccountId == request.ToAccountId)
                {
                    return new TransactionResponse
                    {
                        Success = false,
                        Message = "Cannot transfer to the same account"
                    };
                }

                var transferTransaction = new Transaction
                {
                    FromAccountId = request.FromAccountId,
                    ToAccountId = request.ToAccountId,
                    Amount = request.Amount,
                    Type = TransactionType.Transfer,
                    Status = TransactionStatus.Completed,
                    Description = request.Description ?? $"Transfer to {toAccount.AccountNumber}",
                    ReferenceNumber = GenerateReferenceNumber(),
                    ProcessedAt = DateTime.UtcNow
                };

                // Update balances
                fromAccount.Balance -= request.Amount;
                fromAccount.UpdatedAt = DateTime.UtcNow;

                toAccount.Balance += request.Amount;
                toAccount.UpdatedAt = DateTime.UtcNow;

                _context.Transactions.Add(transferTransaction);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Transfer successful. From: {FromAccountId}, To: {ToAccountId}, Amount: {Amount}",
                    request.FromAccountId, request.ToAccountId, request.Amount);

                return new TransactionResponse
                {
                    TransactionId = transferTransaction.Id,
                    ReferenceNumber = transferTransaction.ReferenceNumber,
                    Success = true,
                    Message = "Transfer successful",
                    NewBalance = fromAccount.Balance
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error processing transfer from {FromAccountId} to {ToAccountId}",
                    request.FromAccountId, request.ToAccountId);

                return new TransactionResponse
                {
                    Success = false,
                    Message = "Transfer failed"
                };
            }
        }

        private string GenerateReferenceNumber()
        {
            return $"TRF{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
    }
}


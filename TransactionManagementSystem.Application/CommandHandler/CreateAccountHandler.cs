using MediatR;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.CommandHandler
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
    {
        private readonly AppDbContext _context;
        private readonly IAccountNumberGenerator _accountNumberGenerator;
        private readonly ILogger<CreateAccountHandler> _logger;

        public CreateAccountHandler(AppDbContext context,
                                    IAccountNumberGenerator accountNumberGenerator,
                                    ILogger<CreateAccountHandler> logger)
        {
            _context = context;
            _accountNumberGenerator = accountNumberGenerator;
            _logger = logger;
        }

        public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                var accountNumber = await _accountNumberGenerator.GenerateAsync();

                var account = new Account
                {
                    AccountNumber = accountNumber,
                    AccountHolderName = request.AccountHolderName,
                    AccountType = request.AccountType,
                    Balance = request.InitialDeposit,
                    UserId = request.UserId
                };

                _context.Accounts.Add(account);

                if (request.InitialDeposit > 0)
                {
                    var depositTransaction = new Transaction
                    {
                        FromAccountId = account.Id,
                        Amount = request.InitialDeposit,
                        Type = TransactionType.Deposit,
                        Status = TransactionStatus.Completed,
                        Description = "Initial deposit",
                        ReferenceNumber = GenerateReferenceNumber(),
                        ProcessedAt = DateTime.UtcNow
                    };

                    _context.Transactions.Add(depositTransaction);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Account created successfully. AccountId: {AccountId}, AccountNumber: {AccountNumber}",
                    account.Id, account.AccountNumber);

                return new CreateAccountResponse
                {
                    AccountId = account.Id,
                    AccountNumber = account.AccountNumber,
                    Success = true,
                    Message = "Account created successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account for user {UserId}", request.UserId);
                return new CreateAccountResponse
                {
                    Success = false,
                    Message = "Failed to create account"
                };
            }
        }

        private string GenerateReferenceNumber()
        {
            return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
    }


}
   


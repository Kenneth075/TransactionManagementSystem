using MediatR;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.CommandHandler
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateAccountCommandHandler> _logger;
        private readonly IAccountNumberGenerator _accountNumberGenerator;

        public CreateAccountCommandHandler(AppDbContext context,
                                           ILogger<CreateAccountCommandHandler> logger,
                                           IAccountNumberGenerator accountNumberGenerator)
        {
            _context = context;
            _logger = logger;
            _accountNumberGenerator = accountNumberGenerator;
        }
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Inside => {nameof(CreateAccountCommand)}");

            try
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogError($"User with ID {request.UserId} not found.");
                    throw new ArgumentException($"User with ID {request.UserId} not found.");
                }
                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    AccountNumber = await _accountNumberGenerator.GenerateAsync(),
                    Balance = 0,
                    Status = AccountStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Accounts.Add(account);
                var result = await _context.SaveChangesAsync(cancellationToken);
                if (result <= 0)
                {
                    _logger.LogError("Failed to create account.");
                    throw new Exception("Failed to create account.");
                }

                _logger.LogInformation($"Account created successfully with ID: {account.Id} for user {request.UserId}");

                return account.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
   
}

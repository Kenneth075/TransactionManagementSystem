using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Query;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.QueryHandler
{
    public class GetAccountDetailsQueryHandler : IRequestHandler<GetAccountDetailsQuery, AccountDetailsResponse>
    {
        
        private readonly AppDbContext _context;
        private readonly ILogger<GetAccountDetailsQueryHandler> _logger;

        public GetAccountDetailsQueryHandler(AppDbContext context, ILogger<GetAccountDetailsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AccountDetailsResponse> Handle(GetAccountDetailsQuery request, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Inside ==> {nameof(GetAccountDetailsQueryHandler)}");

            var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

            if (account == null)
                throw new KeyNotFoundException($"Account with ID {request.AccountId} not found.");

            return new AccountDetailsResponse
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                AccountHolderName = account.AccountHolderName,
                Balance = account.Balance,
                AccountType = account.AccountType,
                Status = account.Status,
                CreatedAt = account.CreatedAt
            };
        }
    }
}

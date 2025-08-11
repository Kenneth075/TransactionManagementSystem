using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Query;
using TransactionManagementSystem.Infrastructure.Caching;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.QueryHandler
{
    public class GetAccountDetailsQueryHandler : IRequestHandler<GetAccountDetailsQuery, AccountDetailsResponse>
    {
        
        private readonly AppDbContext _context;
        private readonly ILogger<GetAccountDetailsQueryHandler> _logger;
        private readonly ICacheService _cacheService;

        public GetAccountDetailsQueryHandler(AppDbContext context, ILogger<GetAccountDetailsQueryHandler> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<AccountDetailsResponse> Handle(GetAccountDetailsQuery request, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Inside ==> {nameof(GetAccountDetailsQueryHandler)}");

            var cacheKey = $"account:{request.AccountId}";

            var cachedAccount = await _cacheService.GetAsync<AccountDetailsResponse>(cacheKey);
            if (cachedAccount != null)
            {
                _logger.LogInformation("Returning account {AccountId} from cache", request.AccountId);
                return cachedAccount;
            }

            var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

            if (account == null)
                throw new KeyNotFoundException($"Account with ID {request.AccountId} not found.");

            var accountResponse = new AccountDetailsResponse
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                AccountHolderName = account.AccountHolderName,
                Balance = account.Balance,
                AccountType = account.AccountType,
                Status = account.Status,
                CreatedAt = account.CreatedAt
            };

            await _cacheService.SetAsync(cacheKey, accountResponse, TimeSpan.FromMinutes(10));
            return accountResponse;
        }
    }
}

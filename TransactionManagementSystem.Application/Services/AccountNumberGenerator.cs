
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.Services
{
    public class AccountNumberGenerator : IAccountNumberGenerator
    {
        private readonly IAppDBContext _appDBContext;
        private const string Prefix = "ACC";

        public AccountNumberGenerator(IAppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<string> GenerateAsync()
        {
            string accountNumber;
            bool exists;
            do
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var randomNumber = new Random().Next(1000, 9999);
                accountNumber = $"{Prefix}{timestamp % 100000000}{randomNumber}";

                exists = _appDBContext.Accounts.Any(a => a.AccountNumber == accountNumber);

            } while (exists);

            return accountNumber;
        }
    }
}

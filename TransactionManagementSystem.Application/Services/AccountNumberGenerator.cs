
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.Services
{
    public class AccountNumberGenerator : IAccountNumberGenerator
    {
        public Task<string> GenerateAsync()
        {
            // Generate a 10-digit account number
            var accountNumber = $"1{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(10, 99)}";
            return Task.FromResult(accountNumber);
        }
    }
}

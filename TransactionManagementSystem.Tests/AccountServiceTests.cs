using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Application.CommandHandler;
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Tests
{
    public class AccountServiceTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

   
        [Fact]
        public async Task CreateAccount_ShouldCreateAccount_WhenValidRequest()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            var mockLogger = new Mock<ILogger<CreateAccountHandler>>();

            mockGenerator.Setup(x => x.GenerateAsync())
                .ReturnsAsync("1234567890");

            var handler = new CreateAccountHandler(context, mockGenerator.Object, mockLogger.Object);

            var command = new CreateAccountCommand
            {
                AccountHolderName = "Kenneth Edoho",
                AccountType = AccountType.Savings,
                UserId = Guid.NewGuid(),
                InitialDeposit = 1000
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("1234567890", result.AccountNumber);

            var account = await context.Accounts.FirstOrDefaultAsync();
            Assert.NotNull(account);
            Assert.Equal(1000, account.Balance);

            var transaction = await context.Transactions.FirstOrDefaultAsync();
            Assert.NotNull(transaction);
            Assert.Equal(TransactionType.Deposit, transaction.Type);
            Assert.Equal(1000, transaction.Amount);
        }

        [Fact]
        public async Task Deposit_ShouldIncreaseBalance_AndAddTransaction()
        {
            using var context = GetInMemoryContext();
            var mockLogger = new Mock<ILogger<DepositHandler>>();

            var account = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "1234567890",
                AccountHolderName = "Kenneth Edoho",
                Balance = 500,
                AccountType = AccountType.Savings,
                Status = AccountStatus.Active,
                UserId = Guid.NewGuid()
            };

            context.Accounts.Add(account);
            await context.SaveChangesAsync();

            var handler = new DepositHandler(context, mockLogger.Object);
            var command = new DepositCommand
            {
                AccountId = account.Id,
                Amount = 200,
                Description = "Test deposit"
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal(700, result.NewBalance);

            var updatedAccount = await context.Accounts.FindAsync(account.Id);
            Assert.Equal(700, updatedAccount!.Balance);

            var transaction = await context.Transactions.FirstOrDefaultAsync();
            Assert.NotNull(transaction);
            Assert.Equal(TransactionType.Deposit, transaction.Type);
            Assert.Equal(200, transaction.Amount);
        }

        [Fact]
        public async Task Withdraw_ShouldFail_WhenInsufficientFunds()
        {
            using var context = GetInMemoryContext();
            var mockLogger = new Mock<ILogger<WithdrawHandler>>();

            var account = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "1234567890",
                AccountHolderName = "Kenneth Edoho",
                Balance = 100,
                AccountType = AccountType.Savings,
                Status = AccountStatus.Active,
                UserId = Guid.NewGuid()
            };

            context.Accounts.Add(account);
            await context.SaveChangesAsync();

            var handler = new WithdrawHandler(context, mockLogger.Object);
            var command = new WithdrawCommand
            {
                AccountId = account.Id,
                Amount = 200,
                Description = "Test withdrawal"
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Insufficient funds", result.Message);

            var updatedAccount = await context.Accounts.FindAsync(account.Id);
            Assert.Equal(100, updatedAccount!.Balance);

            // Make sure no transaction was recorded
            var transactions = await context.Transactions.ToListAsync();
            Assert.Empty(transactions);
        }
    }
}




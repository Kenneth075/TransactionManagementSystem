using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Domain.Exceptions;

namespace TransactionManagementSystem.Tests
{
    public class AccountTest
    {
        [Fact]
        public void Deposit_WithPositiveAmount_ShouldIncreaseBalance()
        {
            // Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 100.00m,
                Status = AccountStatus.Active
            };

            // Act
            account.Deposit(50.00m);

            // Assert
            Assert.Equal(150.00m, account.Balance);
        }

        [Fact]
        public void Deposit_WithNegativeAmount_ShouldThrowException()
        {
            // Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 100.00m,
                Status = AccountStatus.Active
            };

            // Act & Assert
            Assert.Throws<InvalidTransactionException>(() => account.Deposit(-50.00m));
        }

        [Fact]
        public void Withdraw_WithSufficientFunds_ShouldDecreaseBalance()
        {
            // Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 100.00m,
                Status = AccountStatus.Active
            };

            // Act
            account.Withdraw(30.00m);

            // Assert
            Assert.Equal(70.00m, account.Balance);
        }

        [Fact]
        public void Withdraw_WithInsufficientFunds_ShouldThrowException()
        {
            // Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 50.00m,
                Status = AccountStatus.Active
            };

            // Act & Assert
            Assert.Throws<InsufficientFundsException>(() => account.Withdraw(100.00m));
        }

        [Fact]
        public void CanWithdraw_WithSufficientFunds_ShouldReturnTrue()
        {
            // Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 100.00m,
                Status = AccountStatus.Active
            };

            // Act & Assert
            Assert.True(account.CanWithdraw(50.00m));
        }

        [Fact]
        public void CanWithdraw_WithInsufficientFunds_ShouldReturnFalse()
        {
            // Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 50.00m,
                Status = AccountStatus.Active
            };

            // Act & Assert
            Assert.False(account.CanWithdraw(100.00m));
        }
    }

}


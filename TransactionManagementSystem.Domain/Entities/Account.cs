using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Domain.Exceptions;

namespace TransactionManagementSystem.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidTransactionException("Deposit amount must be positive.");
            Balance += amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidTransactionException("Withdrawal amount must be positive.");

            if (Balance < amount)
                throw new InsufficientFundsException($"Insufficient funds. Available balancde is {Balance:C}");
            Balance -= amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool CanWithdraw(decimal amount)
        {
            return Status == AccountStatus.Active && amount > 0 && Balance >= amount;
        }

        //public void Transfer(Account targetAccount, decimal amount)
        //{
        //    if (targetAccount == null)
        //        throw new AccountNotFoundException("Target account not found.");
        //    if (amount <= 0)
        //        throw new InvalidTransactionException("Transfer amount must be positive.");
        //    Withdraw(amount);
        //    targetAccount.Deposit(amount);
        //    UpdatedAt = DateTime.UtcNow;
        //}

    }
}

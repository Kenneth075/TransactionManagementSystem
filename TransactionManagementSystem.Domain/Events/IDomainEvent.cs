using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Domain.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }

    public class AccountCreatedEvent : IDomainEvent
    {
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }

    public class TransactionCompletedEvent : IDomainEvent
    {
        public Guid TransactionId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }

    public class LowBalanceEvent : IDomainEvent
    {
        public Guid AccountId { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal Threshold { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}

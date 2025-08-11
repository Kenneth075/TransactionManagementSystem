using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Domain.Events
{
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
        public Guid Id { get; protected set; } = Guid.NewGuid();
    }

    public class AccountCreatedEvent : DomainEvent
    {
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class TransactionProcessedEvent : DomainEvent
    {
        public Guid TransactionId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
    }

    public class BalanceUpdatedEvent : DomainEvent
    {
        public Guid AccountId { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal NewBalance { get; set; }
    }
}

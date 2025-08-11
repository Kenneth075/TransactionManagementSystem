using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Application.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public string? Description { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public Guid? FromAccountId { get; set; }

    }
}

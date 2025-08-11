using System.ComponentModel.DataAnnotations;
using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string AccountNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string AccountHolderName { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        [Required]
        public AccountStatus Status { get; set; } = AccountStatus.Active;

        [Required]
        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}

using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
        //public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}

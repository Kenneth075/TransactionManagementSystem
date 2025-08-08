using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Application.DTOs
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; } 
        public UserDto User { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}

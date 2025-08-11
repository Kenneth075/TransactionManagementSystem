using MediatR;
using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Application.Query
{
    public class GetAccountDetailsQuery : IRequest<AccountDetailsResponse>
    {
        public Guid AccountId { get; set; }
    }

    public class AccountDetailsResponse
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

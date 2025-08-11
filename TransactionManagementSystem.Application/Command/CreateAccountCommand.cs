using MediatR;
using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Application.Command
{
    public class CreateAccountCommand : IRequest<CreateAccountResponse>
    {
        public string AccountHolderName { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public Guid UserId { get; set; }
        public decimal InitialDeposit { get; set; }
    }

    public class CreateAccountResponse
    {
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

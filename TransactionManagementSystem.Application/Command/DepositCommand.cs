using MediatR;

namespace TransactionManagementSystem.Application.Command
{
    public class DepositCommand : IRequest<TransactionResponse>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? ExternalReference { get; set; }
    }

    public class TransactionResponse
    {
        public Guid TransactionId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal NewBalance { get; set; }
    }

}

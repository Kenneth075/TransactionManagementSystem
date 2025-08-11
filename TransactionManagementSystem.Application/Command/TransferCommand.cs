using MediatR;

namespace TransactionManagementSystem.Application.Command
{
    public class TransferCommand : IRequest<TransactionResponse>
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}

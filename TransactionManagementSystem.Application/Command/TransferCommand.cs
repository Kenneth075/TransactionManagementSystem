using MediatR;

namespace TransactionManagementSystem.Application.Command
{
    public class TransferCommand : IRequest<Guid>
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
    }
}

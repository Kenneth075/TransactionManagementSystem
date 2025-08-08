using MediatR;

namespace TransactionManagementSystem.Application.Command
{
    internal class DepositCommand : IRequest<Guid>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
    }
   
}

using MediatR;

namespace TransactionManagementSystem.Application.Command
{
    public class WithdrawCommand : IRequest<TransactionResponse>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? ExternalReference { get; set; }
    }
  
}

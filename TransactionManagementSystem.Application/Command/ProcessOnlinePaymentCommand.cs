using MediatR;

namespace TransactionManagementSystem.Application.Command
{
    internal class ProcessOnlinePaymentCommand : IRequest<Guid>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string PaymentReference { get; set; }
        public string PaymentProvider { get; set; }
    }

}

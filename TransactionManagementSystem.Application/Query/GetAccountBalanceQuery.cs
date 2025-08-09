using MediatR;

namespace TransactionManagementSystem.Application.Query
{
    public class GetAccountBalanceQuery : IRequest<decimal>
    {
        public Guid AccountId { get; set; }
    }
    
}

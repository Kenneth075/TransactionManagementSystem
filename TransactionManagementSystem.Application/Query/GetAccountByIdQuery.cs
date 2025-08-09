using MediatR;
using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Query
{
    public class GetAccountByIdQuery : IRequest<AccountDto>
    {
        public Guid AccountId { get; set; }
    }
 
}

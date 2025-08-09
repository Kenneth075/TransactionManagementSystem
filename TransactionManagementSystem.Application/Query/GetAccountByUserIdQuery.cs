using MediatR;
using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Query
{
    public class GetAccountByUserIdQuery : IRequest<List<AccountDto>>
    {
        public Guid UserId { get; set; }
    }

}

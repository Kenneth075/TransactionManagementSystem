using MediatR;
using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Query
{
    public class GetTransactionHistoryQuery : IRequest<List<TransactionDto>>
    {
        public Guid AccountId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}

using MediatR;
using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Query
{
    public class GetTransactionHistoryQuery : IRequest<TransactionHistoryResponse>
    {
        public Guid AccountId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class TransactionHistoryResponse
    {
        public List<TransactionDto> Transactions { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

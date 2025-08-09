using MediatR;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Application.Query
{
    public class SerachTransactionsQuery : IRequest<List<TransactionDto>>
    {
        public Guid AccountId { get; set; }
        public string Reference { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}

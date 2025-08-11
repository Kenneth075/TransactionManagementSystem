using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Query;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.QueryHandler
{
    public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, TransactionHistoryResponse>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GetTransactionHistoryQueryHandler> _logger;

        public GetTransactionHistoryQueryHandler(AppDbContext context, ILogger<GetTransactionHistoryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TransactionHistoryResponse> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Inside ==> {nameof(GetTransactionHistoryQueryHandler)}");

            var query = _context.Transactions.AsNoTracking().Where(t => t.FromAccountId == request.AccountId || t.ToAccountId == request.AccountId);

            if (request.FromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(t => t.CreatedAt <= request.ToDate.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var transactions = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    ReferenceNumber = t.ReferenceNumber,
                    Amount = t.Amount,
                    Description = t.Description,
                    Status = t.Status,
                    Type = t.Type,
                    FromAccountId = t.FromAccountId,
                    CreatedAt = t.CreatedAt,
                    ProcessedAt = t.ProcessedAt,
                })
                .ToListAsync(cancellationToken);

            return new TransactionHistoryResponse
            {
                Transactions = transactions,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

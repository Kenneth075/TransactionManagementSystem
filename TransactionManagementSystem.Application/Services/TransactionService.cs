using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAppDBContext _appDBContext;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(IAppDBContext appDBContext,
                                  ILogger<TransactionService> logger)
        {
            _appDBContext = appDBContext;
            _logger = logger;
        }

        public async Task<bool> ProcessTransactionAsync(Guid transactionId)
        {
            _logger.LogInformation($"Inside {nameof(ProcessTransactionAsync)}");

            var transactions = await _appDBContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
            if(transactions == null)
            {
                return false;
            }
            //Update Transaction Status
            transactions.Status = TransactionStatus.Completed;
            transactions.ProcessedAt = DateTime.UtcNow;

            await _appDBContext.SaveChangesAsync();
        }

        public Task<List<TransactionDto>> GetTransactionHistoryAsync(Guid accountId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyStatementDto> GenerateMonthlyStatementAsync(Guid accountId, int month, int year)
        {
            throw new NotImplementedException();
        }

    }
}

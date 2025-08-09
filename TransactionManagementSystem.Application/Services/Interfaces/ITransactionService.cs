using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<bool> ProcessTransactionAsync(Guid transactionId);
        Task<List<TransactionDto>> GetTransactionHistoryAsync(Guid accountId, int pageNumber, int pageSize);
        Task<MonthlyStatementDto> GenerateMonthlyStatementAsync(Guid accountId, int month, int year);

    }
}

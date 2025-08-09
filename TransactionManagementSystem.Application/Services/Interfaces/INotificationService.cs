using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendTransactionNotificationAsync(Guid userId, TransactionDto transaction);
        Task SendLowBalanceAlertAsync(Guid userId, decimal currentBalance, string accountNumber);
        Task SendAccountCreatedNotificationAsync(Guid userId, string accountNumber);

    }
}

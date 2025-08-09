using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Services.Interfaces;

namespace TransactionManagementSystem.Application.Services
{
    public class NotificationService //: INotificationService
    {
        //TODO

        //private readonly ILogger<NotificationService> _logger;

        //public NotificationService(ILogger<NotificationService> logger)
        //{
        //    _logger = logger;
        //}

        //public async Task SendTransactionNotificationAsync(Guid userId, TransactionDto transaction)
        //{
        //    _logger.LogInformation("Sending transaction notification to user {UserId} for transaction {TransactionId}",
        //        userId, transaction.Id);


        //    await Task.CompletedTask;
        //}

        //public async Task SendLowBalanceAlertAsync(Guid userId, decimal currentBalance, string accountNumber)
        //{
        //    _logger.LogWarning("Low balance alert for user {UserId}, account {AccountNumber}, balance: {Balance:C}",
        //        userId, accountNumber, currentBalance);

        //    await Task.CompletedTask;
        //}

        //public async Task SendAccountCreatedNotificationAsync(Guid userId, string accountNumber)
        //{
        //    _logger.LogInformation("Account created notification for user {UserId}, account {AccountNumber}",
        //        userId, accountNumber);

        //    await Task.CompletedTask;
        //}
    }
}


using TransactionManagementSystem.Application.DTOs;
using static TransactionManagementSystem.Application.Services.PaystackService;
using static TransactionManagementSystem.Application.Services.PaystackService.PaystackVerifyData;

namespace TransactionManagementSystem.Application.Services.Interfaces
{
    public interface IPaystackService
    {
        Task<PaystackResponse> InitializePaymentAsync(decimal amount, string email, string reference);
        Task<PaystackResponse> VerifyPaymentAsync(string reference);
        Task<WithdrawalResponses> ProcessWithdrawalAsync(WithdrawalRequests request);
        Task<PaystackResponse> CreateTransferRecipientAsync(string name, string accountNumber, string bankCode);
        Task<PaystackResponse> InitiateTransferAsync(decimal amount, string recipientCode, string reference);


    }
}

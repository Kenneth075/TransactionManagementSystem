using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Services.Interfaces
{
    public interface IPaystackService
    {
        Task<PaystackResponse> InitializePaymentAsync(decimal amount, string email, string reference);
        Task<PaystackResponse> VerifyPaymentAsync(string reference);

    }
}

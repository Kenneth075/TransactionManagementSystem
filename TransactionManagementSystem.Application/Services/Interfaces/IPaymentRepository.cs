using TransactionManagementSystem.Application.DTOs.PaystackDtos;

namespace TransactionManagementSystem.Application.Services.Interfaces
{
    public interface IPaymentRepository
    {
        Task<InitiateResponse> InitializePayment(PaystackInitateRequest request);
        Task<PaystackVerifyResponse> VerifyPayment(string reference);
    }
}

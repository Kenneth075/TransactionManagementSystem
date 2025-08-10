using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Services.Interfaces
{
    public interface IPaymentsService
    {
        Task<PaymentInitializationResponse> InitializePaymentAsync(PaymentRequest request);
        Task<PaymentVerificationResponse> VerifyPaymentAsync(string reference);
        Task<WithdrawalResponse> ProcessWithdrawalAsync(WithdrawalRequest request);

    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Services.Interfaces;

namespace TransactionManagementSystem.Application.Services
{
    public class PaystackService : IPaymentsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaystackService> _logger;
        private readonly string _secretKey;
        private readonly string _baseUrl = "https://api.paystack.co";

        public PaystackService(HttpClient httpClient, IConfiguration configuration, ILogger<PaystackService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _secretKey = configuration["Paystack:SecretKey"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _secretKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<PaymentInitializationResponse> InitializePaymentAsync(PaymentRequest request)
        {
            try
            {
                var payload = new
                {
                    email = request.Email,
                    amount = (int)(request.Amount * 100), // Paystack expects amount in kobo
                    reference = request.Reference,
                    currency = request.Currency ?? "NGN",
                    callback_url = request.CallbackUrl
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/transaction/initialize", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<PaystackResponse<PaymentData>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return new PaymentInitializationResponse
                    {
                        Success = result.Status,
                        Reference = result.Data?.Reference,
                        AuthorizationUrl = result.Data?.AuthorizationUrl,
                        AccessCode = result.Data?.AccessCode,
                        Message = result.Message
                    };
                }

                _logger.LogError("Paystack initialization failed: {Response}", responseContent);
                return new PaymentInitializationResponse { Success = false, Message = "Payment initialization failed" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing payment with Paystack");
                return new PaymentInitializationResponse { Success = false, Message = "Payment service error" };
            }
        }

        public async Task<PaymentVerificationResponse> VerifyPaymentAsync(string reference)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/transaction/verify/{reference}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<PaystackResponse<TransactionData>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return new PaymentVerificationResponse
                    {
                        Success = result.Status,
                        Reference = result.Data?.Reference,
                        Amount = (decimal)(result.Data?.Amount / 100m), // Convert from kobo to naira
                        Status = result.Data?.Status,
                        PaidAt = result.Data?.PaidAt,
                        Message = result.Message
                    };
                }

                return new PaymentVerificationResponse { Success = false, Message = "Verification failed" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment with Paystack");
                return new PaymentVerificationResponse { Success = false, Message = "Verification service error" };
            }
        }

        public async Task<WithdrawalResponse> ProcessWithdrawalAsync(WithdrawalRequest request)
        {
            // Implementation for withdrawal via transfer recipient
            try
            {
                // First create transfer recipient
                var recipientPayload = new
                {
                    type = "nuban",
                    name = request.RecipientName,
                    account_number = request.AccountNumber,
                    bank_code = request.BankCode,
                    currency = request.Currency ?? "NGN"
                };

                var recipientJson = JsonSerializer.Serialize(recipientPayload);
                var recipientContent = new StringContent(recipientJson, Encoding.UTF8, "application/json");

                var recipientResponse = await _httpClient.PostAsync($"{_baseUrl}/transferrecipient", recipientContent);
                var recipientResponseContent = await recipientResponse.Content.ReadAsStringAsync();

                if (!recipientResponse.IsSuccessStatusCode)
                {
                    return new WithdrawalResponse { Success = false, Message = "Failed to create recipient" };
                }

                var recipientResult = JsonSerializer.Deserialize<PaystackResponse<RecipientData>>(recipientResponseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                // Then initiate transfer
                var transferPayload = new
                {
                    source = "balance",
                    amount = (int)(request.Amount * 100),
                    recipient = recipientResult.Data?.RecipientCode,
                    reason = request.Reason ?? "Withdrawal"
                };

                var transferJson = JsonSerializer.Serialize(transferPayload);
                var transferContent = new StringContent(transferJson, Encoding.UTF8, "application/json");

                var transferResponse = await _httpClient.PostAsync($"{_baseUrl}/transfer", transferContent);
                var transferResponseContent = await transferResponse.Content.ReadAsStringAsync();

                if (transferResponse.IsSuccessStatusCode)
                {
                    var transferResult = JsonSerializer.Deserialize<PaystackResponse<TransferData>>(transferResponseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return new WithdrawalResponse
                    {
                        Success = transferResult.Status,
                        TransferCode = transferResult.Data?.TransferCode,
                        Reference = transferResult.Data?.Reference,
                        Message = transferResult.Message
                    };
                }

                return new WithdrawalResponse { Success = false, Message = "Transfer failed" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal with Paystack");
                return new WithdrawalResponse { Success = false, Message = "Withdrawal service error" };
            }
        }
    }
}

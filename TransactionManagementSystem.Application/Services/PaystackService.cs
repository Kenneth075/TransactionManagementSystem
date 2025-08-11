using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Services.Interfaces;
using static TransactionManagementSystem.Application.Services.PaystackService.PaystackVerifyData;

namespace TransactionManagementSystem.Application.Services
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly ILogger<PaystackService> _logger;

        public PaystackService(HttpClient httpClient, IConfiguration configuration, ILogger<PaystackService> logger)
        {
            _httpClient = httpClient;
            _secretKey = configuration["Paystack:SecretKey"] ?? "";
            _logger = logger;

            _httpClient.BaseAddress = new Uri("https://api.paystack.co/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_secretKey}");
        }

        public async Task<PaystackResponse> InitializePaymentAsync(decimal amount, string email, string reference)
        {
            try
            {
                var payload = new
                {
                    amount = (int)(amount * 100), // Paystack expects amount in kobo
                    email,
                    reference,
                    currency = "NGN"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("transaction/initialize", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<PaystackInitializeResponse>(responseContent);
                    return new PaystackResponse
                    {
                        Success = result?.Status == true,
                        Message = result?.Message ?? "",
                        Data = result?.Data
                    };
                }

                return new PaystackResponse
                {
                    Success = false,
                    Message = "Payment initialization failed"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Paystack payment");
                return new PaystackResponse
                {
                    Success = false,
                    Message = "Payment initialization error"
                };
            }
        }

        public async Task<PaystackResponse> VerifyPaymentAsync(string reference)
        {
            try
            {
                var response = await _httpClient.GetAsync($"transaction/verify/{reference}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<PaystackVerifyResponse>(responseContent);
                    return new PaystackResponse
                    {
                        Success = result?.Status == true && result?.Data?.Status == "success",
                        Message = result?.Message ?? "",
                        Data = result?.Data
                    };
                }

                return new PaystackResponse
                {
                    Success = false,
                    Message = "Payment verification failed"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Paystack payment");
                return new PaystackResponse
                {
                    Success = false,
                    Message = "Payment verification error"


                };
            }
            

        }
        public async Task<WithdrawalResponses> ProcessWithdrawalAsync(WithdrawalRequests request)
        {
            try
            {
                // First, create a transfer recipient if not exists
                var recipientResponse = await CreateTransferRecipientAsync(
                    request.AccountName,
                    request.AccountNumber,
                    request.BankCode);

                if (!recipientResponse.Success)
                {
                    return new WithdrawalResponses
                    {
                        Success = false,
                        Message = "Failed to create transfer recipient",
                        Reference = request.Reference
                    };
                }

                // Extract recipient code from response
                var recipientData = JsonSerializer.Deserialize<PaystackRecipientData>(
                    JsonSerializer.Serialize(recipientResponse.Data));

                if (recipientData == null)
                {
                    return new WithdrawalResponses
                    {
                        Success = false,
                        Message = "Invalid recipient data",
                        Reference = request.Reference
                    };
                }

                // Initiate transfer
                var transferResponse = await InitiateTransferAsync(
                    request.Amount,
                    recipientData.Recipient_code,
                    request.Reference);

                return new WithdrawalResponses
                {
                    Success = transferResponse.Success,
                    Message = transferResponse.Message,
                    Reference = request.Reference,
                    TransferCode = recipientData.Recipient_code,
                    Amount = request.Amount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal for reference {Reference}", request.Reference);
                return new WithdrawalResponses
                {
                    Success = false,
                    Message = "Withdrawal processing error",
                    Reference = request.Reference
                };
            }
        }

        public async Task<PaystackResponse> CreateTransferRecipientAsync(string name, string accountNumber, string bankCode)
        {
            try
            {
                var payload = new
                {
                    type = "nuban",
                    name,
                    account_number = accountNumber,
                    bank_code = bankCode,
                    currency = "NGN"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("transferrecipient", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<PaystackRecipientResponse>(responseContent);
                    return new PaystackResponse
                    {
                        Success = result?.Status == true,
                        Message = result?.Message ?? "",
                        Data = result?.Data
                    };
                }

                return new PaystackResponse
                {
                    Success = false,
                    Message = "Failed to create transfer recipient"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transfer recipient");
                return new PaystackResponse
                {
                    Success = false,
                    Message = "Transfer recipient creation error"
                };
            }
        }

        public async Task<PaystackResponse> InitiateTransferAsync(decimal amount, string recipientCode, string reference)
        {
            try
            {
                var payload = new
                {
                    source = "balance",
                    amount = (int)(amount * 100), // Convert to kobo
                    recipient = recipientCode,
                    reference,
                    reason = "Withdrawal request"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("transfer", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<PaystackTransferResponse>(responseContent);
                    return new PaystackResponse
                    {
                        Success = result?.Status == true,
                        Message = result?.Message ?? "",
                        Data = result?.Data
                    };
                }

                return new PaystackResponse
                {
                    Success = false,
                    Message = "Failed to initiate transfer"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating transfer");
                return new PaystackResponse
                {
                    Success = false,
                    Message = "Transfer initiation error"
                };
            }
        }

        public class PaystackResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public object? Data { get; set; }
        }

        public class PaystackInitializeResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; } = string.Empty;
            public PaystackInitializeData? Data { get; set; }
        }

        public class PaystackInitializeData
        {
            public string Authorization_url { get; set; } = string.Empty;
            public string Access_code { get; set; } = string.Empty;
            public string Reference { get; set; } = string.Empty;
        }

        public class PaystackVerifyResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; } = string.Empty;
            public PaystackVerifyData? Data { get; set; }
        }

        public class PaystackVerifyData
        {
            public string Status { get; set; } = string.Empty;
            public string Reference { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public class PaystackRecipientResponse
            {
                public bool Status { get; set; }
                public string Message { get; set; } = string.Empty;
                public PaystackRecipientData? Data { get; set; }
            }

            public class PaystackRecipientData
            {
                public bool Active { get; set; }
                public DateTime CreatedAt { get; set; }
                public string Currency { get; set; } = string.Empty;
                public string Domain { get; set; } = string.Empty;
                public int Id { get; set; }
                public string Name { get; set; } = string.Empty;
                public string Recipient_code { get; set; } = string.Empty;
                public string Type { get; set; } = string.Empty;
                public DateTime UpdatedAt { get; set; }
                public bool Is_deleted { get; set; }
                public PaystackRecipientDetails? Details { get; set; }
            }

            public class PaystackRecipientDetails
            {
                public string Authorization_code { get; set; } = string.Empty;
                public string Account_number { get; set; } = string.Empty;
                public string Account_name { get; set; } = string.Empty;
                public string Bank_code { get; set; } = string.Empty;
                public string Bank_name { get; set; } = string.Empty;
            }

            public class PaystackTransferResponse
            {
                public bool Status { get; set; }
                public string Message { get; set; } = string.Empty;
                public PaystackTransferData? Data { get; set; }
            }

            public class PaystackTransferData
            {
                public int Amount { get; set; }
                public string Currency { get; set; } = string.Empty;
                public string Domain { get; set; } = string.Empty;
                public string Failures { get; set; } = string.Empty;
                public int Id { get; set; }
                public string Integration { get; set; } = string.Empty;
                public string Reason { get; set; } = string.Empty;
                public string Reference { get; set; } = string.Empty;
                public string Source { get; set; } = string.Empty;
                public string Source_details { get; set; } = string.Empty;
                public string Status { get; set; } = string.Empty;
                public string Transfer_code { get; set; } = string.Empty;
                public DateTime CreatedAt { get; set; }
                public DateTime UpdatedAt { get; set; }
            }

            public class WithdrawalRequests
            {
                public string Reference { get; set; } = string.Empty;
                public decimal Amount { get; set; }
                public string AccountName { get; set; } = string.Empty;
                public string AccountNumber { get; set; } = string.Empty;
                public string BankCode { get; set; } = string.Empty;
                public Guid AccountId { get; set; }
                public string Description { get; set; } = string.Empty;
            }

            public class WithdrawalResponses
            {
                public bool Success { get; set; }
                public string Message { get; set; } = string.Empty;
                public string Reference { get; set; } = string.Empty;
                public string? TransferCode { get; set; }
                public decimal Amount { get; set; }
                public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
            }
        }
    
    }
}


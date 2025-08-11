using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Services.Interfaces;

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
        public string Gateway_response { get; set; } = string.Empty;
    }
}


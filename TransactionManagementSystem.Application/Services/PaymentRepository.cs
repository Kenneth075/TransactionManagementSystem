using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PayStack.Net;
using TransactionManagementSystem.Application.DTOs.PaystackDtos;
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.Services
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentRepository> _logger;
        private readonly IConfiguration _config;
        private PayStackApi _payStackApi;
        private readonly string _secretKey;
        public PaymentRepository(AppDbContext context, IConfiguration config, ILogger<PaymentRepository> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _secretKey = _config["Paystack:SecretKey"];
            _payStackApi = new PayStackApi(_secretKey);

        }

        public async Task<InitiateResponse> InitializePayment(PaystackInitateRequest request)
        {
            try
            {
                
                var paystackRequest = new TransactionInitializeRequest
                {
                    AmountInKobo = (int)(decimal.Parse(request.Amount) * 100), // Convert NGN to kobo
                    Email = request.Email,
                    Currency = "NGN",
                    CallbackUrl = _config["Paystack:CallBackUrl"]
                };

                _logger.LogInformation($"Request to Paystack: {JsonConvert.SerializeObject(paystackRequest)}");

                var responseFromPaystack = _payStackApi.Transactions.Initialize(paystackRequest);

                _logger.LogInformation($"Response from Paystack: {JsonConvert.SerializeObject(responseFromPaystack)}");

                if (responseFromPaystack.Status)
                {
                    var payment = new Payment
                    {
                        Amount = decimal.Parse(request.Amount),
                        Email = request.Email,
                        Currency = "NGN",
                        Status = "pending",
                        Reference = responseFromPaystack.Data.Reference,
                        Message = responseFromPaystack.Message
       
                    };

                    await _context.Payments.AddAsync(payment);
                    await _context.SaveChangesAsync();
                }

                return new InitiateResponse
                {
                    Status = responseFromPaystack.Status,
                    Message = responseFromPaystack.Message,
                    AuthorizationUrl = responseFromPaystack.Data?.AuthorizationUrl,
                    AccessCode = responseFromPaystack.Data?.AccessCode,
                    Reference = responseFromPaystack.Data?.Reference,
                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in InitializePayment");
                return new InitiateResponse
                {
                    Status = false,
                    Message = "An error occurred initializing payment"
                };
            }
        }

        public async Task<PaystackVerifyResponse> VerifyPayment(string reference)
        {
            try
            {
                _logger.LogInformation($"Verifying payment with reference: {reference}");

                var verifyResponse = _payStackApi.Transactions.Verify(reference);

                _logger.LogInformation($"Paystack verify response: {JsonConvert.SerializeObject(verifyResponse)}");

                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Reference == reference);

                if (payment == null)
                {
                    _logger.LogWarning($"No payment found in database for reference: {reference}");
                    return new PaystackVerifyResponse
                    {
                        Status = false,
                        Message = "Payment record not found"
                    };
                }

                // Update payment details based on verification
                payment.Status = verifyResponse.Status ? "success" : "failed";
                payment.Message = verifyResponse.Message;
                payment.PaymentMethod = verifyResponse.Data?.Channel;
                payment.Banks = verifyResponse.Data?.Authorization?.Bank;
                payment.PaymentDate = verifyResponse.Data?.TransactionDate ?? DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new PaystackVerifyResponse
                {
                    Status = verifyResponse.Status,
                    Message = verifyResponse.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying payment");
                return new PaystackVerifyResponse
                {
                    Status = false,
                    Message = "Error occurred while verifying payment"
                };
            }
        }

        
    }
}


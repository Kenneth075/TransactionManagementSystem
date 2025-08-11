using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionManagementSystem.Application.DTOs.PaystackDtos;
using TransactionManagementSystem.Application.Services.Interfaces;

namespace TransactionManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
    
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentRepository paymentRepository, ILogger<PaymentController> logger)
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
        }


        [HttpPost("initialize")]
        public async Task<InitiateResponse> InitializeTransaction([FromBody] PaystackInitateRequest request)
        {
            try
            {
                var response = await _paymentRepository.InitializePayment(request);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured", ex.Message);
                return null;
            }
        }

        [HttpGet("verify")]
        public async Task<PaystackVerifyResponse> VerifyTransaction(string reference)
        {
            try
            {
                var response = await _paymentRepository.VerifyPayment(reference);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured", ex.Message);
                return null;
            }
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Data;
using static TransactionManagementSystem.Application.Services.PaystackService.PaystackVerifyData;

namespace TransactionManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaystackService _paymentService;
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaystackService paymentService,
            AppDbContext context,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentRequest request)
        {
            try
            {
                // Verify account exists and is active
                var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == request.AccountId && a.Status == AccountStatus.Active);

                if (account == null)
                {
                    return BadRequest("Account not found or inactive");
                }

                var reference = $"PAY_{DateTime.UtcNow:yyyyMMddHHmmss}_{Random.Shared.Next(1000, 9999)}";

                var response = await _paymentService.InitializePaymentAsync(
                    request.Amount,
                    account.User.Email,
                    reference);

                if (response.Success)
                {
                    var transaction = new Transaction
                    {
                        FromAccountId = request.AccountId,
                        Amount = request.Amount,
                        Type = TransactionType.ExternalPayment,
                        Status = TransactionStatus.Pending,
                        Description = "External payment initialization",
                        ReferenceNumber = reference,
                        ExternalReference = reference
                    };

                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        Success = true,
                        Message = "Payment initialized successfully",
                        Data = response.Data,
                        Reference = reference
                    });
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing payment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("verify/{reference}")]
        public async Task<IActionResult> VerifyPayment(string reference)
        {
            try
            {
                var response = await _paymentService.VerifyPaymentAsync(reference);

                if (response.Success)
                {
                    // Find the pending transaction
                    var transaction = await _context.Transactions
                        .FirstOrDefaultAsync(t => t.ExternalReference == reference &&
                                           t.Status == TransactionStatus.Pending);

                    if (transaction != null)
                    {
                        // Update transaction status and account balance
                        using var dbTransaction = await _context.Database.BeginTransactionAsync();

                        try
                        {
                            transaction.Status = TransactionStatus.Completed;
                            transaction.ProcessedAt = DateTime.UtcNow;

                            var account = await _context.Accounts.FindAsync(transaction.FromAccountId);
                            if (account != null)
                            {
                                account.Balance += transaction.Amount;
                                account.UpdatedAt = DateTime.UtcNow;
                            }

                            await _context.SaveChangesAsync();
                            await dbTransaction.CommitAsync();

                            _logger.LogInformation("Payment verified and processed successfully. Reference: {Reference}", reference);
                        }
                        catch (Exception ex)
                        {
                            await dbTransaction.RollbackAsync();
                            _logger.LogError(ex, "Error processing verified payment");
                            throw;
                        }
                    }
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("processWithdraw")]
        public async Task<ActionResult<WithdrawalResponses>> ProcessWithdrawal([FromBody] WithdrawalRequests request)
        {
            try
            {
                // Verify account exists, is active, and has sufficient balance
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.Status == AccountStatus.Active);

                if (account == null)
                {
                    return BadRequest(new WithdrawalResponse
                    {
                        Success = false,
                        Message = "Account not found or inactive",
                        Reference = request.Reference
                    });
                }

                if (account.Balance < request.Amount)
                {
                    return BadRequest(new WithdrawalResponse
                    {
                        Success = false,
                        Message = "Insufficient funds",
                        Reference = request.Reference
                    });
                }

                // Generate reference if not provided
                if (string.IsNullOrEmpty(request.Reference))
                {
                    request.Reference = $"WTH_{DateTime.UtcNow:yyyyMMddHHmmss}_{Random.Shared.Next(1000, 9999)}";
                }

                // Create pending withdrawal transaction
                var transaction = new Transaction
                {
                    FromAccountId = request.AccountId,
                    Amount = request.Amount,
                    Type = TransactionType.Withdrawal,
                    Status = TransactionStatus.Pending,
                    Description = request.Description ?? "External withdrawal",
                    ReferenceNumber = request.Reference,
                    ExternalReference = request.Reference
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                // Process withdrawal through Paystack
                var response = await _paymentService.ProcessWithdrawalAsync(request);

                if (response.Success)
                {
                    // Update transaction status and deduct from account balance
                    using var dbTransaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        transaction.Status = TransactionStatus.Completed;
                        transaction.ProcessedAt = DateTime.UtcNow;

                        account.Balance -= request.Amount;
                        account.UpdatedAt = DateTime.UtcNow;

                        await _context.SaveChangesAsync();
                        await dbTransaction.CommitAsync();

                        response.Amount = request.Amount;
                        _logger.LogInformation("Withdrawal processed successfully. Reference: {Reference}", request.Reference);
                    }
                    catch (Exception ex)
                    {
                        await dbTransaction.RollbackAsync();
                        _logger.LogError(ex, "Error processing withdrawal transaction");

                        response.Success = false;
                        response.Message = "Failed to update account balance";
                    }
                }
                else
                {
                    // Mark transaction as failed
                    transaction.Status = TransactionStatus.Failed;
                    await _context.SaveChangesAsync();
                }

                if (response.Success)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal");
                return StatusCode(500, new WithdrawalResponse
                {
                    Success = false,
                    Message = "Internal server error",
                    Reference = request.Reference
                });
            }
        }

        [HttpGet("banks")]
        public async Task<IActionResult> GetBanks()
        {
            try
            {
                // This would typically call Paystack's bank list API
                // For now, return a static list of major Nigerian banks
                var banks = new[]
                {
                    new { Name = "Access Bank", Code = "044" },
                    new { Name = "Guaranty Trust Bank", Code = "058" },
                    new { Name = "United Bank For Africa", Code = "033" },
                    new { Name = "Zenith Bank", Code = "057" },
                    new { Name = "First Bank of Nigeria", Code = "011" },
                    new { Name = "Fidelity Bank", Code = "070" },
                    new { Name = "FCMB", Code = "214" },
                    new { Name = "Sterling Bank", Code = "232" },
                    new { Name = "Union Bank", Code = "032" },
                    new { Name = "Wema Bank", Code = "035" }
                };

                return Ok(new { Success = true, Data = banks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bank list");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class InitializePaymentRequest
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}


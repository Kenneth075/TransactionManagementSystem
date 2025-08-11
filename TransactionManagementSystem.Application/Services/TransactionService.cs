using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.DTOs;
using TransactionManagementSystem.Application.Services.Interfaces;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Caching;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.Services
{
    public class TransactionService 
    {
        //private readonly AppDbContext _appDBContext;
        //private readonly ILogger<TransactionService> _logger;
        //private readonly ICacheService _cacheService;

        //public TransactionService(AppDbContext appDBContext,
        //                          ILogger<TransactionService> logger,
        //                          ICacheService cacheService)
        //{
        //    _appDBContext = appDBContext;
        //    _logger = logger;
        //    _cacheService = cacheService;
        //}

        //public async Task<bool> ProcessTransactionAsync(Guid transactionId)
        //{
        //    _logger.LogInformation($"Inside {nameof(ProcessTransactionAsync)}");

        //    var transactions = await _appDBContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
        //    if(transactions == null)
        //    {
        //        return false;
        //    }
        //    //Update Transaction Status
        //    transactions.Status = TransactionStatus.Completed;
        //    transactions.ProcessedAt = DateTime.UtcNow;

        //    await _appDBContext.SaveChangesAsync();

        //    //Clear Cache from account

        //    await _cacheService.RemoveAsync($"Account Balance_{transactions.FromAccountId}");

        //    if(transactions.ToAccountId.HasValue)
        //    {
        //        await _cacheService.RemoveAsync($"Account Balance_ {transactions.ToAccountId}");
        //    }

        //    _logger.LogInformation($"Transaction {transactionId} processed successfully.");
        //    return true;
        //}

        //public async Task<List<TransactionDto>> GetTransactionHistoryAsync(Guid accountId, int pageNumber, int pageSize)
        //{
        //    var cacheKey = $"Transaction History_{accountId}_{pageNumber}_{pageSize}";
        //    var CacheTransactions = await _cacheService.GetAsync<List<TransactionDto>>(cacheKey);

        //    if(CacheTransactions != null)
        //    {
        //        _logger.LogInformation($"Cache hit for {cacheKey}");
        //        return CacheTransactions;
        //    }

        //    var transactions = await _appDBContext.Transactions
        //        .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
        //        .OrderByDescending(t => t.CreatedAt)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(t => new TransactionDto
        //        {
        //            Id = t.Id,
        //            FromAccountId = t.FromAccountId,
        //            ToAccountId = t.ToAccountId,
        //            Amount = t.Amount,
        //            Status = t.Status,
        //            Type = t.Type,
        //            Description = t.Description,
        //            Reference = t.Reference,
        //            CreatedAt = t.CreatedAt,
        //            ProcessedAt = t.ProcessedAt
        //        })
        //        .ToListAsync();

        //    await _cacheService.SetAsync(cacheKey, transactions, TimeSpan.FromMinutes(5));

        //    return transactions;
        //}

        //public async Task<MonthlyStatementDto> GenerateMonthlyStatementAsync(Guid accountId, int month, int year)
        //{
        //    var startDate = new DateTime(year, month, 1);
        //    var endDate = startDate.AddMonths(1).AddDays(-1);

        //    _logger.LogInformation($"Generating monthly statement for Account ID: {accountId}, Month: {month}, Year: {year}");

        //    var transactions = await _appDBContext.Transactions
        //        .Where(t => t.FromAccountId == accountId && t.CreatedAt >= startDate && t.CreatedAt <= endDate && t.Status == TransactionStatus.Completed)
        //        .OrderBy(t => t.CreatedAt)
        //        .Select(t => new TransactionDto
        //        {
        //            Id = t.Id,
        //            FromAccountId = t.FromAccountId,
        //            ToAccountId = t.ToAccountId,
        //            Amount = t.Amount,
        //            Status = t.Status,
        //            Type = t.Type,
        //            Description = t.Description,
        //            Reference = t.Reference,
        //            CreatedAt = t.CreatedAt,
        //            ProcessedAt = t.ProcessedAt
        //        }).ToListAsync();

        //    var account = await _appDBContext.Accounts.Include(a => a.User).FirstOrDefaultAsync();

        //    var credits = transactions.Where(t => t.ToAccountId == accountId).Sum(t => t.Amount);
        //    var debits = transactions.Where(t => t.FromAccountId == accountId).Sum(t => t.Amount);

        //    return new MonthlyStatementDto
        //    {
        //        AccountId = accountId,
        //        AccountNumber = account?.AccountNumber,
        //        Month = month,
        //        Year = year,
        //        OpeningBalance = account?.Balance - credits + debits ?? 0,
        //        ClosingBalance = account?.Balance ?? 0,
        //        Transactions = transactions,
        //        TotalTransactions = transactions.Count,
        //        TotalCredits = credits,
        //        TotalDebits = debits

        //    };
        //}

    }
}

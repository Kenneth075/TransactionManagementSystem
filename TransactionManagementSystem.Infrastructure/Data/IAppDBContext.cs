using Microsoft.EntityFrameworkCore;
using TransactionManagementSystem.Domain.Entities;

namespace TransactionManagementSystem.Infrastructure.Data
{
    public interface IAppDBContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

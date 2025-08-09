using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionManagementSystem.Domain.Entities;

namespace TransactionManagementSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext, IAppDBContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
        }

        public class UserConfiguration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.HasKey(u => u.Id);

                builder.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                builder.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(20);

                builder.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                builder.Property(u => u.PhoneNumber)
                    .HasMaxLength(20);

                builder.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                builder.HasIndex(u => u.Email).IsUnique();

                builder.HasMany(u => u.Accounts)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class AccountConfiguration : IEntityTypeConfiguration<Account>
        {
            public void Configure(EntityTypeBuilder<Account> builder)
            {
                builder.HasKey(a => a.Id);

                builder.Property(a => a.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                builder.Property(a => a.Balance)
                    .HasPrecision(18, 2);

                builder.HasIndex(a => a.AccountNumber).IsUnique();

                builder.HasMany(a => a.Transactions)
                    .WithOne(t => t.FromAccount)
                    .HasForeignKey(t => t.FromAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
        {
            public void Configure(EntityTypeBuilder<Transaction> builder)
            {
                builder.HasKey(t => t.Id);

                builder.Property(t => t.Amount)
                    .HasPrecision(18, 2);

                builder.Property(t => t.Description)
                    .HasMaxLength(500);

                builder.Property(t => t.Reference)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.HasIndex(t => t.Reference);
                builder.HasIndex(t => t.CreatedAt);

                builder.HasOne(t => t.ToAccount)
                    .WithMany()
                    .HasForeignKey(t => t.ToAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}

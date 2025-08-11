using Microsoft.Extensions.DependencyInjection;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Domain.Enums;
using TransactionManagementSystem.Infrastructure.Data;
using TransactionManagementSystem.Infrastructure.Security;

namespace TransactionManagementSystem.Infrastructure.Seeding
{
    public static class DataSeeding
    {
        public static async Task SeedDatabaseAsync(AppDbContext appDbContext,IServiceProvider serviceProvider)
        {
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();

            if (!appDbContext.Users.Any())
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@bankingsystem.com",
                    PhoneNumber = "+2348138933344",
                    PasswordHash = passwordHasher.HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var testUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Kenneth",
                    LastName = "Edoho",
                    Email = "kenneth.edoho@bankingsystem.com",
                    PhoneNumber = "+2348138933344",
                    PasswordHash = passwordHasher.HashPassword("Test123!"),
                    Role = UserRole.Customer,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                appDbContext.Users.AddRange(adminUser, testUser);

                var testAccount = new Account
                {
                    Id = Guid.NewGuid(),
                    UserId = testUser.Id,
                    AccountNumber = "1000000001",
                    Balance = 1000.00m,
                    Status = AccountStatus.Active,
                    AccountType = AccountType.Savings,
                    AccountHolderName = "Kenneth Edoho",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                appDbContext.Accounts.Add(testAccount);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
    
}

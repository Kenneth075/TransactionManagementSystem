using Microsoft.EntityFrameworkCore;
using TransactionManagementSystem.Infrastructure.Data;
using TransactionManagementSystem.Infrastructure.Seeding;

namespace TransactionManagementSystem.API.Extensions
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (app.Environment.IsDevelopment())
            {
                context.Database.EnsureCreated();
                await DataSeeding.SeedDatabaseAsync(context, scope.ServiceProvider);
            }
            else
            {
                context.Database.Migrate();
            }
        }
    }
}


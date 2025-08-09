namespace TransactionManagementSystem.Application.Services.Interfaces
{
    public interface IAccountNumberGenerator
    {
        Task<string> GenerateAsync();
    }
}

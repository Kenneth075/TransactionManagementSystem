namespace TransactionManagementSystem.Application.DTOs
{
    public class MonthlyStatementDto
    {
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<TransactionDto> Transactions { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }

    }
}

using MediatR;
using TransactionManagementSystem.Application.DTOs;

namespace TransactionManagementSystem.Application.Query
{
    public class GetMonthlyStatementQuery : IRequest<MonthlyStatementDto>
    {
        public Guid AccountId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
  
}

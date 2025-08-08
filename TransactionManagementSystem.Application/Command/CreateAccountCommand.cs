using MediatR;

namespace TransactionManagementSystem.Application.Command
{
    public class CreateAccountCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
    }
}

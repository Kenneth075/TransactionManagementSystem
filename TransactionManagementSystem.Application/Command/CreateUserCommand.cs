using MediatR;
using TransactionManagementSystem.Domain.Enums;

namespace TransactionManagementSystem.Application.Command
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
    }
    
}

using MediatR;
using Microsoft.Extensions.Logging;
using TransactionManagementSystem.Application.Command;
using TransactionManagementSystem.Domain.Entities;
using TransactionManagementSystem.Infrastructure.Data;

namespace TransactionManagementSystem.Application.CommandHandler
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAppDBContext _context;
        private readonly ILogger<CreateAccountCommandHandler> _logger;

        public CreateAccountCommandHandler(IAppDBContext context,
                                           ILogger<CreateAccountCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Inside => {nameof(CreateAccountCommand)}");

            try
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogError($"User with ID {request.UserId} not found.");
                    throw new ArgumentException($"User with ID {request.UserId} not found.");
                }
                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    AccountNumber = 
                    
                };
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
   
}

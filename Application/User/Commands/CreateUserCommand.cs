using MediatR;

namespace Application.User.Commands
{
    public class CreateUserCommand : IRequest<decimal>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}

using MediatR;

namespace Application.Auth.Commands.LogoutCommand
{
    public class LogoutCommand : IRequest
    {
        public required int UserId { get; set; }
        public required string Jti { get; set; }
    }
}

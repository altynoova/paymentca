using MediatR;

namespace Application.Auth.Commands.LoginCommand;

public class LoginCommand : IRequest<string>
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}
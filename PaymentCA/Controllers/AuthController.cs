using Application.Auth.Commands.LoginCommand;
using Application.Auth.Commands.LogoutCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginCommand command) => Ok(await _mediator.Send(command));

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var jti = User.GetJti();
            await _mediator.Send(new LogoutCommand { UserId = userId, Jti = jti }, ct);

            return NoContent();
        }
    }
}

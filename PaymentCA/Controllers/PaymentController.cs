using Application.Payment.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentController(IMediator mediator) => _mediator = mediator;

        [HttpPost("charge")]
        public async Task<ActionResult<decimal>> Charge(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var balanceCents = await _mediator.Send(new MakePaymentCommand { UserId = userId }, ct);
            return Ok(balanceCents);
        }
    }
}

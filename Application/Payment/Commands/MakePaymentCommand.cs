using MediatR;

namespace Application.Payment.Commands
{
    public class MakePaymentCommand : IRequest<decimal>
    {
        public required int UserId { get; set; }
    }
}

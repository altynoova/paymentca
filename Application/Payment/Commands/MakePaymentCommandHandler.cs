using Application.Common.Interfaces;
using MediatR;

namespace Application.Payment.Commands
{
    public class MakePaymentHandler : IRequestHandler<MakePaymentCommand, decimal>
    {
        private readonly IAppDbContext _db;
        private const decimal ChargeCents = 110;

        public MakePaymentHandler(IAppDbContext db) => _db = db;

        public async Task<decimal> Handle(MakePaymentCommand r, CancellationToken ct)
        {
            var newBalance = await _db.TryDebitBalanceAsync(r.UserId, ChargeCents, ct) ?? throw new InvalidOperationException("Insufficient funds.");
            _db.Payments.Add(new Domain.Entities.Payment { UserId = r.UserId, AmountCents = ChargeCents });
            await _db.SaveChangesAsync(ct);

            return newBalance /100m;
        }
    }
}

using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.LogoutCommand
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IAppDbContext _db;
        public LogoutCommandHandler(IAppDbContext db) => _db = db;

        public async Task Handle(LogoutCommand r, CancellationToken ct)
        {
            var sess = await _db.Sessions.FirstOrDefaultAsync(s => s.UserId == r.UserId && s.Jti == r.Jti, ct);

            if (sess is null) return;

            if (sess.RevokedAt is null)
            {
                sess.RevokedAt = DateTimeOffset.UtcNow;
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}

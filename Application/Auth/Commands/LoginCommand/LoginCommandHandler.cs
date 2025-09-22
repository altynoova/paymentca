using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Application.Auth.Commands.LoginCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IJwtTokenService _jwt;
        private readonly IAppDbContext _db;
        IOptions<JwtOptions> _opts;
        public LoginCommandHandler(UserManager<ApplicationUser> users, IJwtTokenService jwt, IAppDbContext db, IOptions<JwtOptions> opts)
        {
            _users = users; _jwt = jwt; _db = db; _opts = opts; 
        }

        public async Task<string> Handle(LoginCommand r, CancellationToken ct)
        {
            var user = await _users.Users.FirstOrDefaultAsync(u => u.UserName == r.Login, ct);

            if (user is null)
            {
                await LogAttempt(r.Login, success: false, "Invalid credentials", ct);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (await _users.IsLockedOutAsync(user))
            {
                await LogAttempt(r.Login, success: false, "Locked out", ct);
                throw new UnauthorizedAccessException("Account is locked. Try later.");
            }

            var ok = await _users.CheckPasswordAsync(user, r.Password);
            if (!ok)
            {
                await _users.AccessFailedAsync(user);
                var reason = await _users.IsLockedOutAsync(user) ? "Locked out" : "Invalid credentials";
                await LogAttempt(r.Login, success: false, reason, ct);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            await _users.ResetAccessFailedCountAsync(user);
            await LogAttempt(r.Login, success: true, "OK", ct);

            var lifetime = TimeSpan.FromMinutes(_opts.Value.AccessMinutes);
            var issuer = _opts.Value.Issuer!;
            var audience = _opts.Value.Audience!;
            var key = _opts.Value.Secret!;
            var token = _jwt.IssueToken(user.Id, user.UserName!, out var jti, lifetime, issuer, audience, key);

            _db.Sessions.Add(new Session
            {
                UserId = user.Id,
                Jti = jti,
                ExpiresAt = DateTimeOffset.UtcNow.Add(lifetime)
            });
            await _db.SaveChangesAsync(ct);

            return token;
        }

        private async Task LogAttempt(string username, bool success, string reason, CancellationToken ct)
        {
            _db.LoginAttempts.Add(new LoginAttempt
            {
                Username = username,
                Success = success,
                Reason = reason,
            });
            await _db.SaveChangesAsync(ct);
        }
    }
}

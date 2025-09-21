using Application.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        public string IssueToken(int userId, string username, out string jti, TimeSpan lifetime,
                                 string issuer, string audience, string signingKey)
        {
            jti = Guid.NewGuid().ToString("N");
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer, audience, claims, now, now.Add(lifetime), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}

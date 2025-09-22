using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var raw = user.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(raw, out var userId))
                throw new UnauthorizedAccessException("Invalid token subject.");

            return userId;
        }

        public static string GetJti(this ClaimsPrincipal user)
        {
            var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (string.IsNullOrEmpty(jti))
                throw new UnauthorizedAccessException("Token does not contain jti.");
            return jti;
        }
    }
}

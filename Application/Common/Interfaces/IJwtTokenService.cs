namespace Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string IssueToken(int userId, string username, out string jti, TimeSpan lifetime,
                          string issuer, string audience, string signingKey);
    }
}

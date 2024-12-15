using System.Security.Claims;

namespace Documents
{
    public interface IJwtAuth
    {
        string Authentication(string username, string password);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string RGenerateAccessToken(string username);
        string RGenerateAccessTokenClaims(IEnumerable<Claim> claims);
    }
}

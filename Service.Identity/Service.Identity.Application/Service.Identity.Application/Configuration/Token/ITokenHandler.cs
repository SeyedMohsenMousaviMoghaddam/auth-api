using System.Security.Claims;
using Service.Identity.Application.Identity.Contracts;
using Service.Identity.Domain.Users;

namespace Service.Identity.Application.Configuration.Token;

public interface ITokenHandler
{
    public IdentityTokenResponseModel Generate(List<Claim> authClaims, bool temp, bool canUseRefreshToken);
    public bool IsValidToken(string token, bool temp);
    public IdentityTokenDataResponseModel ReadToken(string bearerToken);
    public List<Claim> GenerateClaims(User user, List<string>? roles = null, List<string>? policies = null);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
}
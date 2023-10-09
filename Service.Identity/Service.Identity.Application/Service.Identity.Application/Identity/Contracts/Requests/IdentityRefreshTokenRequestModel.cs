using Service.Identity.Application.Common;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityRefreshTokenRequest : IContract
{
    public string Token { get; set; }
    public string? RefreshToken { set; get; }
}
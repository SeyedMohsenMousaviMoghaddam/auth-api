#nullable enable
using Service.Identity.Application.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityTokenResponseModel : IContract
{
    public string? BranchColor { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { set; get; }
    public DateTime? Expiration { get; set; }
    [SwaggerSchema("username")]
    public string? UserName { get; set; }
    [SwaggerSchema("TimeBase")]
    public DateTimeOffset TimeBase { get; set; }
    [SwaggerSchema("expiration date")]
    public DateTimeOffset? ExpirationDate { get; set; }
    public bool TwoFactorEnabled { get; set; } = false;

}
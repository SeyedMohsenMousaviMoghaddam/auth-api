using Service.Identity.Domain.Common;
using System.Security.Claims;

namespace Service.Identity.Api.Util;
public class UserInfo : IUserInfo
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserInfo(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? UserId
    {
        get
        {
            var userId = Convert.ToInt64(User?.FindFirst("nameidentifier")?.Value);
            return userId;
        }
    }

    public string? Token => _httpContextAccessor?.HttpContext?.Request.Headers.Authorization.ToString();

    public ClaimsPrincipal User => _httpContextAccessor?.HttpContext?.User;
}
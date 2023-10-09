using Service.Identity.Application.Common;
using Service.Identity.Domain.Users;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityForgotPasswordResponseModel : IMapping<User>, IContract
{
    [SwaggerSchema("username")]
    public string UserName { get; set; }
    [SwaggerSchema("TimeBase")]
    public DateTimeOffset TimeBase { get; set; }
    [SwaggerSchema("expiration date")]
    public DateTimeOffset ExpirationDate { get; set; }
}
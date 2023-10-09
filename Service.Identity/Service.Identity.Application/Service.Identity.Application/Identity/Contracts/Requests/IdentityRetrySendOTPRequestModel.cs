using Service.Identity.Application.Common;
using Service.Identity.Domain.Users;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityRetrySendOTPRequest : IMapping<User>, IContract
{
    [SwaggerSchema("username")]
    public string UserName { get; set; }
}
using Service.Identity.Application.Common;
using Service.Identity.Domain.Users;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityVerifyOTPRequest : IMapping<User>, IContract
{
    [SwaggerSchema("username")]
    public string UserName { get; set; }

    [SwaggerSchema("otp code")]
    public string OTPCode { get; set; }
}
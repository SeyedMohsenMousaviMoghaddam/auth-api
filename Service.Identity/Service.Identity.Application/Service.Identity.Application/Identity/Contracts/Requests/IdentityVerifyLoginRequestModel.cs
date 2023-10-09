using Service.Identity.Application.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityVerifyLoginRequestModel : IContract
{
    [SwaggerSchema("username")]
    public string UserName { get; set; }

    [SwaggerSchema("otp code")]
    public string OTPCode { get; set; }
    public string Password { get; set; }
}
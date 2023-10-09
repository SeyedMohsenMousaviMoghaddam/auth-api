using Service.Identity.Application.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityVerifyForgotPasswordRequestModel : IContract
{
    [SwaggerSchema("username")]
    public string UserName { get; set; }

    [SwaggerSchema("otp code")]
    public string OTPCode { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}
using Service.Identity.Application.Common;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityForgotPasswordRequestModel : IContract
{
    public string UserName { get; set; }
}
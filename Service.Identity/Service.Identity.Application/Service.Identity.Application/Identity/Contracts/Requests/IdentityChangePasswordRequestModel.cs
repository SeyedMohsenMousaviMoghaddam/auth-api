using Service.Identity.Application.Common;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityChangePasswordRequestModel : IContract
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}
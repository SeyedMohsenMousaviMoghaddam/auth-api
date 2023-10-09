using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserPolcies.Contracts;

public class UserHasPolicyResponseModel : IContract
{
    public bool UserHasPolicy { get; set; }
}
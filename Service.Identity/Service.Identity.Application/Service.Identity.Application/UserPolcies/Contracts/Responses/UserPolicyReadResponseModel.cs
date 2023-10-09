
using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserPolcies.Contracts;

public class UserPolicyReadResponseModel : IContract
{
    public string Policies { get; set; }
}
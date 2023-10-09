using Service.Identity.Application.Common;

namespace Service.Identity.Application.RolePolicies.Contracts;

public class RolePolicyDeleteRequestModel : IContract
{
    public long RoleId { get; set; }
    public long[] PoliciesIds { get; set; }
}
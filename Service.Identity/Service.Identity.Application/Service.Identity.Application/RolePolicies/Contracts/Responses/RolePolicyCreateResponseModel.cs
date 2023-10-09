using Service.Identity.Application.Common;
using Service.Identity.Domain.RolePolicies;

namespace Service.Identity.Application.RolePolicies.Contracts;

public class RolePolicyCreateResponseModel : IMapping<RolePolicy>, IContract
{
    public long Id { get; set; }
}
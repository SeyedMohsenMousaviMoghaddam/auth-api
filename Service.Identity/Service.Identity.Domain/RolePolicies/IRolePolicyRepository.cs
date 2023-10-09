using Service.Identity.Domain.Common;

namespace Service.Identity.Domain.RolePolicies;

public interface IRolePolicyRepository : IGenericRepository<RolePolicy, long>
{
}
using Service.Identity.Domain.RolePolicies;
using Service.Identity.Infrastructure.Configuration;

namespace Service.Identity.Infrastructure.RolePolicies;

public class RolePolicyRepository : GenericRepository<RolePolicy, long, IdentityContext>, IRolePolicyRepository
{
    public RolePolicyRepository(IdentityContext dbContext) : base(dbContext)
    {
    }
}
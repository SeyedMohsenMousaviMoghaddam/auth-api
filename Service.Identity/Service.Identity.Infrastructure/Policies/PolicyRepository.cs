using Service.Identity.Domain.Policies;
using Service.Identity.Infrastructure.Configuration;

namespace Service.Identity.Infrastructure.Policies;

public class PolicyRepository : GenericRepository<Policy, long, IdentityContext>, IPolicyRepository
{
    public PolicyRepository(IdentityContext dbContext) : base(dbContext)
    {
    }
}
using Service.Identity.Domain.UserPolcies;
using Service.Identity.Infrastructure.Configuration;

namespace Service.Identity.Infrastructure.UserPolicies;

public class UserPolicyRepository : GenericRepository<UserPolicy, long, IdentityContext>, IUserPolicyRepository
{
    public UserPolicyRepository(IdentityContext dbContext) : base(dbContext)
    {
    }
}
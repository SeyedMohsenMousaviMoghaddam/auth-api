using Service.Identity.Domain.UserRoles;
using Service.Identity.Infrastructure.Configuration;

namespace Service.Identity.Infrastructure.UserRoles;

public class UserRoleRepository : GenericRepository<UserRole, long, IdentityContext>, IUserRoleRepository
{
    public UserRoleRepository(IdentityContext dbContext) : base(dbContext)
    {
    }
}
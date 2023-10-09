using Service.Identity.Domain.Roles;
using Service.Identity.Infrastructure.Configuration;

namespace Service.Identity.Infrastructure.Roles;

public class RoleRepository : GenericRepository<Role, long, IdentityContext>, IRoleRepository
{
    public RoleRepository(IdentityContext dbContext) : base(dbContext)
    {
    }
}
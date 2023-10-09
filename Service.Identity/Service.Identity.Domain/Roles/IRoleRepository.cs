using Service.Identity.Domain.Common;

namespace Service.Identity.Domain.Roles;

public interface IRoleRepository : IGenericRepository<Role, long>
{
}
using Service.Identity.Domain.Common;

namespace Service.Identity.Domain.UserRoles;

public interface IUserRoleRepository : IGenericRepository<UserRole, long>
{
}
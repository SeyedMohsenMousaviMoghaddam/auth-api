using Service.Identity.Application.Common;
using Service.Identity.Domain.UserRoles;

namespace Service.Identity.Application.UserRoles.Contracts;

public class UserRoleReadResponseModel : IMapping<UserRole>, IContract
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public Guid? TenantId { get; set; }
}
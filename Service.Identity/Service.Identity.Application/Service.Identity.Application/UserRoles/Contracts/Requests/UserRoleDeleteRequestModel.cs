using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserRoles.Contracts;

public class UserRoleDeleteRequestModel : IContract
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public Guid? TenantId { get; set; }
}
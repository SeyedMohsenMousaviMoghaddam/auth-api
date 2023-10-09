using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserRoles.Contracts;

public class UserRoleCreateRequestModel : IContract
{
    public long UserId { get; set; }
    public long[] RoleIds { get; set; }
}
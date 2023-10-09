using Service.Identity.Application.Common;
using Service.Identity.Domain.UserRoles;

namespace Service.Identity.Application.UserRoles.Contracts;

public class UserRoleCreateResponseModel : IMapping<UserRole>, IContract
{
    public long Id { get; set; }
}
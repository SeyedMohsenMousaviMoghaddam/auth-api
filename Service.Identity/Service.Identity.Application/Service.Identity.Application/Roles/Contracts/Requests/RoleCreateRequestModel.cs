using Service.Identity.Application.Common;
using Service.Identity.Domain.Roles;

namespace Service.Identity.Application.Roles.Contracts;

public class RoleCreateRequestModel : IMapping<Role>, IContract
{
    public string Name { get; set; }
}
using Service.Identity.Application.Common;

namespace Service.Identity.Application.Roles.Contracts;

public class RoleUpdateRequestModel : IContract
{
    public long Id { get; set; }
    public string Name { get; set; }
}
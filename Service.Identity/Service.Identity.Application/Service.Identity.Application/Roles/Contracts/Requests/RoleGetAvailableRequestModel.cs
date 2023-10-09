using Service.Identity.Application.Common;

namespace Service.Identity.Application.Roles.Contracts;

public class RoleGetAvailableRequestModel : IContract
{
    public long UserId { get; set; }
}
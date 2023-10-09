using Service.Identity.Application.Common;

namespace Service.Identity.Application.Roles.Contracts;

public class RoleGetRequestModel : IContract
{
    public long Id { get; set; }
}
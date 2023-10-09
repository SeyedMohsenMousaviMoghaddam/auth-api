using Service.Identity.Application.Common;

namespace Service.Identity.Application.RolePolicies.Contracts;

public class RolePolicyGetRequestModel : PaginationRequest<RolePolicyAdvancedFilterRequest>, IContract
{
    public long RoleId { get; set; }
}
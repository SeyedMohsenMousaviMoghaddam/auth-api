using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserPolcies.Contracts;

public class UserPolicyGetPaginatedRequestModel : PaginationRequest<UserPolicyAdvancedFilterRequest>, IContract
{
    public long UserId { get; set; }
    public Guid? TenantId { get; set; }
}
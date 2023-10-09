using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserRoles.Contracts;

public class UserRoleGetPaginatedRequestModel : PaginationRequest<UserRoleAdvancedFilterRequestModel>, IContract
{
    public long UserId { get; set; }
    public Guid? TenantId { get; set; }
}
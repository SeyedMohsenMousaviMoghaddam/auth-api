using Service.Identity.Application.Common;

namespace Service.Identity.Application.Roles.Contracts;

public class RoleGetAllRequestModel : PaginationRequest<RoleAdvancedFilterRequest>, IContract
{
}
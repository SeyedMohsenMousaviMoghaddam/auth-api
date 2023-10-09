using Service.Identity.Application.Common;

namespace Service.Identity.Application.Users.Contracts;

public class UserGetPaginatedRequestModel : PaginationRequest<UserAdvancedFilterRequestModel>, IContract
{
}
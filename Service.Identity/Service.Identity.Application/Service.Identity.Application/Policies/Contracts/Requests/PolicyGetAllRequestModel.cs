using Service.Identity.Application.Common;

namespace Service.Identity.Application.Policies.Contracts;

public class PolicyGetAllRequestModel : PaginationRequest<PolicyAdvancedFilterRequest>, IContract
{
}
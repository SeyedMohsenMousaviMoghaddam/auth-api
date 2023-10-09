using Service.Identity.Application.Common;

namespace Service.Identity.Application.Policies.Contracts;

public class PoliciesReadResponseModel : IContract
{
    public string ApiResource { get; set; }
    public List<ApiScopesListResponseModel> ApiScopes { get; set; }
}
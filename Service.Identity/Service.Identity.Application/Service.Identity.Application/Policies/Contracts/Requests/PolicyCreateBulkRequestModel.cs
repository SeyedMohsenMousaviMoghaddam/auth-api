using Service.Identity.Application.Common;

namespace Service.Identity.Application.Policies.Contracts;

public class PolicyCreateBulkRequestModel : IContract
{
    public string ApiResource { get; set; }
    public PolicyCreateApiScopeRequestModel[] ApiScopes { get; set; }
}

public partial class PolicyCreateApiScopeRequestModel : IContract
{
    public string ApiScope { get; set; }
    public PolicyCreateRequestModel[] Policies { get; set; }
}

public partial class PolicyCreateRequestModel : IContract
{
    public string Policy { get; set; }
    public byte Order { get; set; }
    public byte? Grade { get; set; }
}
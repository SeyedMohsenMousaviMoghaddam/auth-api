using Service.Identity.Application.Common;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentityTokenDataResponseModel : IContract
{
    public string username { get; set; }
    public string sub { get; set; }
    public string roles { get; set; }
    public string tenantId { get; set; }
    public string branchId { get; set; }
    public Dictionary<string, string> attrs { get; set; }
    public string[] UserPolicies { get; set; }
}
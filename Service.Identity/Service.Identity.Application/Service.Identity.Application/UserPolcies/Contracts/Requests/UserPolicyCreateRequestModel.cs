
using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserPolcies.Contracts;

public class UserPolicyCreateRequestModel : IContract
{
    public long UserId { get; set; }
    public long[] PolicyIds { get; set; }
    public Guid TenantId { get; set; }
    public long? TenantId2 { set; get; }
}
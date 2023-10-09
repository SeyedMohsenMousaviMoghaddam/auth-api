using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserPolcies.Contracts;

public class UserHasPolicyRequestModel : IContract
{
    public long UserId { get; set; }
    public string[] Policies { get; set; }
    public Guid? TenantId { get; set; }
    public long? TenantId2 { get; set; }
    public long? BranchId { get; set; }
    public bool Condition { get; set; }
}
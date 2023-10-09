using Service.Identity.Application.Common;

namespace Service.Identity.Application.UserPolcies.Contracts;

public class UserPolicyDeleteRequestModel : IContract
{
    public long UserId { get; set; }
    public long PolicyId { get; set; }
    public Guid? TenantId { get; set; }
    public long? BranchId { get; set; }
}
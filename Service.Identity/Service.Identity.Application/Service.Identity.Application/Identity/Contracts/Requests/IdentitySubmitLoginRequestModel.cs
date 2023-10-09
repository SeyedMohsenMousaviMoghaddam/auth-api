using Service.Identity.Application.Common;

namespace Service.Identity.Application.Identity.Contracts;

public class IdentitySubmitLoginRequestModel : IContract
{
    public Guid? TenantId { get; set; }
    public long? TenantId2 { get; set; }
    public long? BranchId { get; set; }
}
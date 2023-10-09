using Service.Identity.Application.Common;

namespace Service.Identity.Application.Policies.Contracts;

public class PolicyDeleteRequestModel : IContract
{
    public long PolicyId { get; set; }
}
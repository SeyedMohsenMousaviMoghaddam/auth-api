using Service.Identity.Application.Common;

namespace Service.Identity.Application.Policies.Contracts;

public class PolicyGetRequestModel : IContract
{
    public long Id { get; set; }
}
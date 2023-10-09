using Service.Identity.Application.Common;
using Service.Identity.Domain.Policies;

namespace Service.Identity.Application.Policies.Contracts;

public class PolicyResponseModel : IMapping<Policy>, IContract
{
    public long Id { get; set; }
    public string ApiResource { get; set; }
    public string ApiScope { get; set; }
    public string Value { get; set; }
    public byte Order { get; set; }
    public string PolicyValue => $"{ApiResource}.{ApiScope}.{Value}";
}
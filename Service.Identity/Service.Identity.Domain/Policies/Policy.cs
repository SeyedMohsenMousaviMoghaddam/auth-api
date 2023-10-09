using Service.Identity.Domain.Common;
using Service.Identity.Domain.RolePolicies;
using Service.Identity.Domain.UserPolcies;

namespace Service.Identity.Domain.Policies;

public class Policy : Entity
{
    public string ApiResource { get; set; }
    public string ApiScope { get; set; }
    public string Value { get; set; }
    public byte Order { get; set; }
    public byte Grade { get; set; }

    #region Relationship
    private readonly List<RolePolicy> _rolePolicies = new();
    private readonly List<UserPolicy> _userPolicies = new();

    public IReadOnlyCollection<RolePolicy> RolePolicies => _rolePolicies.AsReadOnly();
    public IReadOnlyCollection<UserPolicy> UserPolicies => _userPolicies.AsReadOnly();
    #endregion
}
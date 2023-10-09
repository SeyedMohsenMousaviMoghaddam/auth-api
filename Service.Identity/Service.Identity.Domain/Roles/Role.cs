using Service.Identity.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Service.Identity.Domain.RolePolicies;
using Service.Identity.Domain.UserRoles;
using Service.Identity.Domain.Users;

namespace Service.Identity.Domain.Roles;

public class Role : IdentityRole<long>, IEntity<long>
{
    public override string Name { get; set; }
    public Guid? TenantId { get; set; }
    public long? TenantId2 { set; get; }
    public long CreatedById { get; set; }
    public long? ModifiedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public byte Grade { get; set; }

    public void RemoveRolePolicy(long policyId)
    {
        var rolePolicy = RolePolicies.FirstOrDefault(x => x.PolicyId.Equals(policyId));
        if (rolePolicy is not null)
            _rolePolicies.Remove(rolePolicy);
    }

    #region Relationship
    private readonly List<RolePolicy> _rolePolicies = new();
    private readonly List<UserRole> _userRoles = new();
    
    public IReadOnlyCollection<RolePolicy> RolePolicies => _rolePolicies.AsReadOnly();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    public User CreatedUser { set; get; }
    public User? ModifiedUser { set; get; }
    #endregion
}
using Service.Identity.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Service.Identity.Domain.RolePolicies;
using Service.Identity.Domain.Roles;
using Service.Identity.Domain.UserPolcies;
using Service.Identity.Domain.UserRoles;

namespace Service.Identity.Domain.Users;

public class User : IdentityUser<long>, IEntity<long>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public override string UserName { get; set; }
    public override string Email { get; set; }
    public override string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string Grade { get; set; }
    public long? CreatedById { get; set; }
    public long? ModifiedById { get; set; }
    public long? ActiveChangeStatusBy { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? ActiveChangeStatusDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
    public bool CanUseRefreshToken { set; get; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string LastApplicationVersion { get; set; }
    public void RemoveUserPolicy(long policyId, Guid? tenantId)
    {
        var userPolicy = _userPolicies.FirstOrDefault(x => x.PolicyId.Equals(policyId) && x.TenantId.Equals(tenantId));
        if (userPolicy is not null) _userPolicies.Remove(userPolicy);
    }

    #region Relationship

    public User? CreatedUser { set; get; }
    public User? ModifiedUser { set; get; }

    private readonly List<UserRole> _userRoles = new();
    private readonly List<UserPolicy> _userPolicies = new();
    private readonly List<Role> _createdRoles = new();
    private readonly List<Role> _modifiedRoles = new();
    private readonly List<RolePolicy> _createdRolePolicies = new();
    private readonly List<RolePolicy> _modifiedRolePolicies = new();
    private readonly List<UserPolicy> _createdUserPolicies = new();
    private readonly List<UserPolicy> _modifiedUserPolicies = new();
    private readonly List<UserRole> _createdUserRoles = new();
    private readonly List<UserRole> _modifiedUserRoles = new();


    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
    public IReadOnlyCollection<UserPolicy> UserPolicies => _userPolicies.AsReadOnly();
    public IEnumerable<Role> CreatedRoles => _createdRoles;
    public IEnumerable<Role> ModifiedRoles => _modifiedRoles;
    public IEnumerable<RolePolicy> CreatedRolePolicies => _createdRolePolicies;
    public IEnumerable<RolePolicy> ModifiedRolePolicies => _modifiedRolePolicies;
    public IEnumerable<UserPolicy> CreatedUserPolicies => _createdUserPolicies;
    public IEnumerable<UserPolicy> ModifiedUserPolicies => _modifiedUserPolicies;
    public IEnumerable<UserRole> CreatedUserRoles => _createdUserRoles;
    public IEnumerable<UserRole> ModifiedUserRoles => _modifiedUserRoles;

    #endregion
}
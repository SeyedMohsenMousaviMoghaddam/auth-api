using Service.Identity.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Service.Identity.Domain.Roles;
using Service.Identity.Domain.Users;

namespace Service.Identity.Domain.UserRoles;

public class UserRole : IdentityUserRole<long>, IEntity<long>
{
    public long Id { get; }
    public Guid? TenantId { get; set; }
    public long? TenantId2 { set; get; }
    public long CreatedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public long? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }

    #region Relationship
    public User User { get; set; }
    public User CreatedUser { set; get; }
    public User? ModifiedUser { set; get; }
    public Role Role { get; set; }
    #endregion
}
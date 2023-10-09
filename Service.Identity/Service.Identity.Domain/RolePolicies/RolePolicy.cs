using Service.Identity.Domain.Common;
using Service.Identity.Domain.Policies;
using Service.Identity.Domain.Roles;
using Service.Identity.Domain.Users;

namespace Service.Identity.Domain.RolePolicies;

public class RolePolicy : LoggableEntity
{
    public long RoleId { get; set; }
    public long PolicyId { get; set; }

    #region Relationship
    public Policy Policy { get; set; }
    public Role Role { get; set; }
    public User CreatedUser { set; get; }
    public User? ModifiedUser { set; get; }
    #endregion
}
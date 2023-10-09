using Service.Identity.Domain.Common;
using Service.Identity.Domain.Policies;
using Service.Identity.Domain.Users;

namespace Service.Identity.Domain.UserPolcies;

public class UserPolicy : LoggableEntity
{
    public long PolicyId { get; set; }
    public Guid TenantId { get; set; }
    public long? TenantId2 { set; get; }
    public long UserId { get; set; }

    #region Relationship
    public Policy Policy { get; set; }
    public User User { get; set; }
    public User CreatedUser { set; get; }
    public User? ModifiedUser { set; get; }
    #endregion
}
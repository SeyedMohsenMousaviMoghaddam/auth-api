using Service.Identity.Application.Common;
using Service.Identity.Domain.Users;

namespace Service.Identity.Application.Users;

public class UserSeedResponseModel : IMapping<User>, IContract
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}
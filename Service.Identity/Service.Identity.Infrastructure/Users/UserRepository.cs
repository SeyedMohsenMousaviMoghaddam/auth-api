using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain.Users;
using Service.Identity.Infrastructure.Util;
using Service.Identity.Infrastructure.Configuration;

namespace Service.Identity.Infrastructure.Users;

public class UserRepository : GenericRepository<User, long, IdentityContext>, IUserRepository
{
    // private readonly IConfiguration _configuration;
    // private readonly IUserInfo _userInfo;

    public UserRepository(IdentityContext dbContext
        ) :
        base(dbContext)
    {
        // _configuration = configuration;
        // _userInfo = userInfo;
    }
    public async Task<bool> IsDuplicated(string phoneNumber, string userName, CancellationToken cancellationToken)
    {
        return await Entities.ExcludeSoftDelete()
            .AnyAsync(x => x.UserName.Equals(userName) || x.PhoneNumber.Equals(phoneNumber), cancellationToken);
    }


}
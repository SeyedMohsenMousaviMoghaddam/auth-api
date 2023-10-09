using Service.Identity.Domain.Common;

namespace Service.Identity.Domain.Users;

public interface IUserRepository : IGenericRepository<User, long>
{
    Task<bool> IsDuplicated(string phoneNumber, string userName, CancellationToken cancellationToken);

}
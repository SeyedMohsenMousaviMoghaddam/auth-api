using Service.Identity.Domain.Common;

namespace Service.Identity.Domain.UserPolcies;

public interface IUserPolicyRepository : IGenericRepository<UserPolicy, long>
{
}
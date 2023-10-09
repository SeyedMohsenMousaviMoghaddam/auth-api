using Service.Identity.Domain.Common;

namespace Service.Identity.Domain.Policies;

public interface IPolicyRepository : IGenericRepository<Policy, long>
{
}
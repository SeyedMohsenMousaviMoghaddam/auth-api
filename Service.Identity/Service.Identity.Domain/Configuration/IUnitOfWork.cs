using Service.Identity.Domain.Policies;
using Service.Identity.Domain.RolePolicies;
using Service.Identity.Domain.Roles;
using Service.Identity.Domain.UserPolcies;
using Service.Identity.Domain.UserRoles;
using Service.Identity.Domain.Users;

namespace Service.Identity.Domain.Configuration;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IPolicyRepository Policies { get; }
    IRolePolicyRepository RolePolicies { get; }
    IUserRoleRepository UserRoles { get; }
    IUserPolicyRepository UserPolicies { get; }
    Task CompleteTask();
}
using Service.Identity.Domain.Configuration;

using Service.Identity.Domain.Policies;
using Service.Identity.Domain.RolePolicies;
using Service.Identity.Domain.Roles;
using Service.Identity.Domain.UserPolcies;
using Service.Identity.Domain.UserRoles;
using Service.Identity.Domain.Users;

namespace Service.Identity.Infrastructure.Configuration;

internal class UnitOfWork : IUnitOfWork
{
    private readonly IdentityContext _identityContext;
    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }
    public IPolicyRepository Policies { get; }
    public IRolePolicyRepository RolePolicies { get; }
    public IUserRoleRepository UserRoles { get; }
    public IUserPolicyRepository UserPolicies { get; }


    public UnitOfWork(IdentityContext identityContext,
        IUserRepository users,
        IRoleRepository roles,
        IPolicyRepository policies,
        IRolePolicyRepository rolePolicies,
        IUserRoleRepository userRoles,
        IUserPolicyRepository userPolicies)
    {
        _identityContext = identityContext;
        Users = users;
        Roles = roles;
        Policies = policies;
        RolePolicies = rolePolicies;
        UserRoles = userRoles;
        UserPolicies = userPolicies;
    }

    public async Task CompleteTask()
    {
        await _identityContext.SaveChangesAsync();
    }

    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
            if (disposing)
                _identityContext.Dispose();

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
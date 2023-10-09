using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain.Roles;
using Service.Identity.Infrastructure.Interceptor;
using Service.Identity.Domain.Users;
using Service.Identity.Infrastructure.Configuration;
using Service.Identity.Domain.UserRoles;
using Service.Identity.Domain;
using Service.Identity.Domain.Common;

namespace Service.Identity.Infrastructure;

public class IdentityContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
{
    private readonly IUserInfo _userInfo;

    public IdentityContext(DbContextOptions<IdentityContext> options, IUserInfo userInfo): base(options)
    {
        _userInfo = userInfo;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new IdentitySaveChangesInterceptor(_userInfo));

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().ToTable(TableConsts.IdentityRoles);
        modelBuilder.Entity<UserRole>().ToTable(TableConsts.IdentityUserRoles);
        modelBuilder.Entity<User>().ToTable(TableConsts.IdentityUsers);
        modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable(TableConsts.IdentityRoleClaims);
        modelBuilder.Entity<IdentityUserLogin<long>>().ToTable(TableConsts.IdentityUserLogins);
        modelBuilder.Entity<IdentityUserClaim<long>>().ToTable(TableConsts.IdentityUserClaims);
        modelBuilder.Entity<IdentityUserToken<long>>().ToTable(TableConsts.IdentityUserTokens);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
    }
}
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;
using Service.Identity.Domain.Common;

namespace Service.Identity.Infrastructure.Interceptor
{
    internal class IdentitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IUserInfo _userInfo;

        public IdentitySaveChangesInterceptor(IUserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            foreach (var entry in eventData.Context.ChangeTracker.Entries<LoggableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        entry.Entity.ModifiedById = _userInfo.UserId;
                        entry.Entity.ModifiedDate = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedById = _userInfo.UserId;
                        entry.Entity.ModifiedDate = DateTime.Now;
                        break;
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedById = _userInfo.UserId ?? default(long);
                        break;
                    default:
                        break;
                }
            }
            
            foreach (var entry in eventData.Context.ChangeTracker.Entries<LoggableEntity<Guid>>())
            {
                switch (entry.State)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        entry.Entity.ModifiedById = _userInfo.UserId;
                        entry.Entity.ModifiedDate = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedById = _userInfo.UserId;
                        entry.Entity.ModifiedDate = DateTime.Now;
                        break;
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedById = _userInfo.UserId ?? default(long);
                        break;
                    default:
                        break;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesFailedAsync(eventData, cancellationToken);
        }
    }
}
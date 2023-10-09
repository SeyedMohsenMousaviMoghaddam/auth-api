using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain.Common;

namespace Service.Identity.Infrastructure.Configuration;
public class GenericRepository<TEntity, TKey, TDbContext> : IGenericRepository<TEntity, TKey>
            where TEntity : class, IEntity<TKey> where TDbContext : DbContext
{
    #region construction

    protected readonly TDbContext DbContext;
    public DbSet<TEntity> Entities { get; }
    public virtual IQueryable<TEntity> Table => Entities;
    public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

    public GenericRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
        Entities = DbContext.Set<TEntity>();
    }

    #endregion

    #region async method

    public virtual Task<IQueryable<TEntity>> SearchAsync(Guid? tenantId, long? branchId, string? filter, int top)
    {
        throw new NotImplementedException();
    }

    public virtual ValueTask<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
    {
        return Entities.FindAsync(ids, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken,
        bool saveNow = true)
    {
        var result = await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken, bool saveNow = true)
    {
        await Entities.AddRangeAsync(entities, cancellationToken);

        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken);

        return entities;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
    {
        Entities.Update(entity);

        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<TEntity> UpdateAsyncWithResult(TEntity entity, CancellationToken cancellationToken,
        bool saveNow = true)
    {
        var result = Entities.Update(entity);

        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result.Entity;
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken,
        bool saveNow = true)
    {
        Entities.UpdateRange(entities);

        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
    {
        Entities.Remove(entity);

        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken,
        bool saveNow = true)
    {
        Entities.RemoveRange(entities);

        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<bool> IsExistsAsync(TKey id, CancellationToken cancellationToken)
    {
        var isExists = await Entities.AsNoTracking().AnyAsync(f => f.Id.Equals(id), cancellationToken);

        return isExists;
    }

    #endregion

    #region sync methods

    public virtual TEntity GetById(params object[] ids)
    {
        return Entities.Find(ids);
    }

    public virtual void Add(TEntity entity, bool saveNow = true)
    {
        Entities.Add(entity);

        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
    {
        Entities.AddRange(entities);

        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void Update(TEntity entity, bool saveNow = true)
    {
        Entities.Update(entity);

        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
    {
        Entities.UpdateRange(entities);

        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void Delete(TEntity entity, bool saveNow = true)
    {
        Entities.Remove(entity);

        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
    {
        Entities.RemoveRange(entities);

        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual bool IsExists(TKey id)
    {
        var isExists = Entities.Any(f => f.Id.Equals(id));

        return isExists;
    }

    #endregion
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Domain.Common
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        #region construction

        DbSet<TEntity> Entities { get; }
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> TableNoTracking { get; }

        #endregion

        #region sync method

        TEntity GetById(params object[] ids);
        void Add(TEntity entity, bool saveNow = true);
        void AddRange(IEnumerable<TEntity> entities, bool saveNow = true);
        void Update(TEntity entity, bool saveNow = true);
        void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true);
        void Delete(TEntity entity, bool saveNow = true);
        bool IsExists(TKey id);

        #endregion

        #region async method

        Task<IQueryable<TEntity>> SearchAsync(Guid? tenantId, long? branchId, string filter, int top);
        ValueTask<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);

        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken,
            bool saveNow = true);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);

        Task<TEntity> UpdateAsyncWithResult(TEntity entity, CancellationToken cancellationToken,
            bool saveNow = true);

        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);

        Task<bool> IsExistsAsync(TKey id, CancellationToken cancellationToken);

        #endregion
    }
}

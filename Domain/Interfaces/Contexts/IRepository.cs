using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkPaginateCore;

namespace Domain.Interfaces.Contexts
{
    public interface IRepository<TEntity> where TEntity : class, IBaseEntity
    {
        Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes);
        Task<TEntity> FirstByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes);
        Task<TEntity> FirstOrDefaultAsync(bool AsNoTracking = true, params string[] includes);
        Task<IEnumerable<TEntity>> GetAllAsync(bool AsNoTracking = true, params string[] includes);
        Task<TEntity> GetAsync(object id, bool AsNoTracking = true, params string[] includes);
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> InsertAsync(TEntity entity, bool SaveChange = false, CancellationToken cancellationToken = default);
        Task<TEntity> LastByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes);
        Task<TEntity> LastOrDefaultAsync(bool AsNoTracking = true, params string[] includes);
        Task<Page<TEntity>> PaginateAsync(int page, int count, Sorts<TEntity> sorts, Filters<TEntity> filters, bool AsNoTracking = true, params string[] includes);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<TEntity> SoftDeleteAsync(object id, bool SaveChange = false, CancellationToken cancellationToken = default);
        IQueryable<TEntity> GetRawTable();
    }
}

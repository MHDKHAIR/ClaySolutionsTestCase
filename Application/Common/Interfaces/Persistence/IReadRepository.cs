using System;
using Domain.Interfaces;
using EntityFrameworkPaginateCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;

namespace Application.Common.Interfaces.Persistence
{
    public interface IReadRepository<TEntity> where TEntity : class, IBaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool AsNoTracking = true, params string[] includes);
        Task<Page<TEntity>> PaginateAsync(int page, int count, Sorts<TEntity> sorts, Filters<TEntity> filters, bool AsNoTracking = true, params string[] includes);
        Task<TEntity> GetAsync(object id, bool AsNoTracking = true, params string[] includes);
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> expression);

        Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes);
        Task<TEntity> FirstByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes);
        Task<TEntity> FirstOrDefaultAsync(bool AsNoTracking = true, params string[] includes);
        Task<TEntity> LastByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes);
        Task<TEntity> LastOrDefaultAsync(bool AsNoTracking = true, params string[] includes);
        IQueryable<TEntity> GetRawTable();
    }
}

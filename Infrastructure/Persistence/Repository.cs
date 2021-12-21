using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Interfaces.Contexts;
using Domain.Interfaces;
using EntityFrameworkPaginateCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Domain.Enums;

namespace Infrastructure.Persistence
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IBaseEntity
    {
        protected ApplicationDbContext _context;
        private DbSet<TEntity> dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = context.Set<TEntity>();
        }

        #region Get
        public async Task<TEntity> GetAsync(object id, bool AsNoTracking = true, params string[] includes)
        {
            TEntity include = default;
            var orginal = await dbSet.FindAsync(id);
            if (orginal is null) return null;

            var set = dbSet.AsQueryable();
            if (AsNoTracking)
                set = dbSet.AsNoTracking();

            if (includes == null) return orginal;

            foreach (var item in includes)
            {
                if (item != null)
                    set = set.Include(item);
            }
            include = await set.FirstOrDefaultAsync(x => x == orginal);
            if (include is null)
                return null;

            return include;
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await dbSet.Where(expression).CountAsync();
        }

        #endregion

        #region GetAll
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool AsNoTracking = true, params string[] includes)
        {
            return await GetSet(AsNoTracking, includes).ToListAsync();
        }
        #endregion

        #region Find
        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes)
        {
            return await GetSet(AsNoTracking, includes).Where(expression).ToListAsync();
        }
        public IEnumerable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes)
        {
            var set = dbSet.AsQueryable();
            if (AsNoTracking)
                set = dbSet.AsNoTracking();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    if (include != null)
                        set = set.Include(include);
                }
            }
            return set.Where(expression);
        }
        #endregion

        #region First
        public async Task<TEntity> FirstByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes)
        {
            return await GetSet(AsNoTracking, includes).FirstOrDefaultAsync(expression);
        }
        public async Task<TEntity> FirstOrDefaultAsync(bool AsNoTracking = true, params string[] includes)
        {
            return await GetSet(AsNoTracking, includes).FirstOrDefaultAsync();
        }
        #endregion

        #region LastOrDefault
        public async Task<TEntity> LastByConditionAsync(Expression<Func<TEntity, bool>> expression, bool AsNoTracking = true, params string[] includes)
        {
            return await GetSet(AsNoTracking, includes).OrderByDescending(x => x.CreatedAt).LastOrDefaultAsync(expression);
        }

        public async Task<TEntity> LastOrDefaultAsync(bool AsNoTracking = true, params string[] includes)
        {
            return await GetSet(AsNoTracking, includes).LastOrDefaultAsync();
        }

        #endregion

        #region Insert

        public async Task<TEntity> InsertAsync(TEntity entity, bool SaveChange = false, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ApplicationException("No Data Found");
            }
            await dbSet.AddAsync(entity);

            if (SaveChange) await SaveChangesAsync(cancellationToken);

            return entity;
        }

        #endregion

        #region DeleteOrRemove
        public async Task<TEntity> SoftDeleteAsync(object id, bool SaveChange = false, CancellationToken cancellationToken = default)
        {
            TEntity entity = await dbSet.FindAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new ApplicationException("No Data Found");
            }
            entity.RecordStatus = RecordStatusEnum.Deleted;

            if (SaveChange) await SaveChangesAsync(cancellationToken);

            return entity;
        }
        #endregion

        #region SaveChanges

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        #endregion

        #region PaginateAsync
        public async Task<Page<TEntity>> PaginateAsync(int page, int count, Sorts<TEntity> sorts, Filters<TEntity> filters, bool AsNoTracking = true, params string[] includes)
        {
            return await GetSet(AsNoTracking, includes).PaginateAsync(page, count < 0 ? int.MaxValue : count, sorts, filters);
        }

        #endregion

        #region Private
        private IQueryable<TEntity> GetSet(bool AsNoTracking, params string[] includes)
        {
            var set = dbSet.AsQueryable();
            if (AsNoTracking)
                set = dbSet.AsNoTracking();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    if (include != null)
                        set = set.Include(include);
                }
            }
            return set;
        }
        #endregion
        #region Table
        public IQueryable<TEntity> GetRawTable() => dbSet.AsNoTracking().AsQueryable();
        #endregion
    }
}

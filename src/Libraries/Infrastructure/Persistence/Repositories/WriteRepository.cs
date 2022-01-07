using Application.Common.Interfaces.Persistence;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class WriteRepository<TEntity> : IWriteRepository<TEntity> where TEntity : class, IBaseEntity
    {
        protected ApplicationDbContext _context;
        private DbSet<TEntity> dbSet;
        public WriteRepository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = context.Set<TEntity>();
        }

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
    }
}

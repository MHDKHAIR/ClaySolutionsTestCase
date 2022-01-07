using Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Persistence
{
    public interface IWriteRepository<TEntity> where TEntity : class, IBaseEntity
    {
        Task<TEntity> InsertAsync(TEntity entity, bool SaveChange = false, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<TEntity> SoftDeleteAsync(object id, bool SaveChange = false, CancellationToken cancellationToken = default);
    }
}

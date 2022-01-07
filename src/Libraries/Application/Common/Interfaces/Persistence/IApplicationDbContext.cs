using System.Threading;
using System.Threading.Tasks;


namespace Application.Common.Interfaces.Persistence
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

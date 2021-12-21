using System.Threading.Tasks;
using Application.DataTransfareObjects.Requests;

namespace Application.Services.Contracts
{
    public interface ILockAccessService
    {
        Task AccessLock(AccessLockRequestDto requestDto);
        Task GrantAccessOnLock(string claimId);
    }
}

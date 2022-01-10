using System.Threading.Tasks;
using Application.DataTransfareObjects.Requests;
using Application.DataTransfareObjects.Responses;

namespace Application.Services.Interfaces
{
    public interface ILockAccessService
    {
        Task<AccessLockResponseDto> AccessLock(AccessLockRequestDto requestDto);
        Task GrantAccessOnLock(string claimId);
    }
}

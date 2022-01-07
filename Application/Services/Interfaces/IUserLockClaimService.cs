using System.Threading.Tasks;
using Application.Common.Enums;
using Application.Common.Results;
using Application.Common.Search;
using Application.DataTransfareObjects.Responses;

namespace Application.Services.Interfaces
{
    public interface IUserLockClaimService
    {
        Task<DataList<UserLocksResponseDto>> UserDookLockListAsync(BaseSearchQuery<UserDoorLocksSearch, GeneralSortEnum> query);
    }
}

using System.Threading.Tasks;
using Application.DataTransfareObjects.Responses;
using Domain.Common.Results;
using Domain.Common.Search;
using Domain.Enums;

namespace Application.Services.Interfaces
{
    public interface IUserLockClaimService
    {
        Task<DataList<UserLocksResponseDto>> UserDookLockListAsync(BaseSearchQuery<UserDoorLocksSearch, GeneralSortEnum> query);
    }
}

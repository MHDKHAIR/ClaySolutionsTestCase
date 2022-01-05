using Application.DataTransfareObjects.Responses;
using Domain.Common.Results;
using Domain.Common.Search;
using Domain.Enums;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ILockAccessHistoryService
    {
        Task<DataList<UserAccessHistortLocksResponseDto>> UserDookLockHistoryListAsync(BaseSearchQuery<UserAccessLocksHistorySearch, GeneralSortEnum> query);
    }
}

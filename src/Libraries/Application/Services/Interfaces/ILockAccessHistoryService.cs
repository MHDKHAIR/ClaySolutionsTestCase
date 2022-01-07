using Application.Common.Enums;
using Application.Common.Results;
using Application.Common.Search;
using Application.DataTransfareObjects.Responses;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ILockAccessHistoryService
    {
        Task<DataList<UserAccessHistortLocksResponseDto>> UserDookLockHistoryListAsync(BaseSearchQuery<UserAccessLocksHistorySearch, GeneralSortEnum> query);
    }
}

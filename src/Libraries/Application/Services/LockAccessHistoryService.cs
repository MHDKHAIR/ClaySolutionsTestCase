using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Persistence;
using Application.Common.Results;
using Application.Common.Search;
using Application.DataTransfareObjects.Responses;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using EntityFrameworkPaginateCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LockAccessHistoryService : ILockAccessHistoryService
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEfRepository<LockAccessHistoryEntity> _accessHistoryRepo;

        public LockAccessHistoryService(IMapper mapper, ICurrentUserService currentUserService,
            IEfRepository<LockAccessHistoryEntity> accessHistoryRepo)
        {
            _mapper = mapper;
            _currentUserService = currentUserService;
            _accessHistoryRepo = accessHistoryRepo;
        }

        public async Task<DataList<UserAccessHistortLocksResponseDto>> UserDookLockHistoryListAsync(BaseSearchQuery<UserAccessLocksHistorySearch, GeneralSortEnum> query)
        {
            var filters = new Filters<LockAccessHistoryEntity>();
            if (query.Search is null) query.Search = new UserAccessLocksHistorySearch();

            bool nameSearchHasValue = !string.IsNullOrEmpty(query.Search.Name);
            filters.Add(nameSearchHasValue, x => x.DoorLock.DoorName.Contains(query.Search.Name));
            filters.Add(query.Search.LastAccessed.HasValue, x => x.CreatedAt == query.Search.LastAccessed.Value);
            filters.Add(query.Search.LockAccessStatus.HasValue, x => x.AccessStatus == query.Search.LockAccessStatus.Value);

            filters.Add(true, x => x.RecordStatus != RecordStatusEnum.Deleted);
            filters.Add(true, x => x.UserId == _currentUserService.UserId);

            var sorts = new Sorts<LockAccessHistoryEntity>();
            sorts.Add(query.Sort == GeneralSortEnum.Id, x => x.Id, query.Order == GeneralOrderEnum.Desc);
            sorts.Add(query.Sort == GeneralSortEnum.Date, x => x.CreatedAt, query.Order == GeneralOrderEnum.Desc);
            var page = await _accessHistoryRepo.PaginateAsync(query.CurrentPage, query.PageSize, sorts, filters, includes: "DoorLock.Floor.Building");
            return new DataList<UserAccessHistortLocksResponseDto>()
            {
                Results = _mapper.Map<IEnumerable<UserAccessHistortLocksResponseDto>>(page.Results),
                PageSize = page.PageSize,
                PageCount = page.PageCount,
                CurrentPage = page.CurrentPage,
                Total = page.RecordCount
            };
        }

    }
}

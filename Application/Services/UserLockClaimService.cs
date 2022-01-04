using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Persistence;
using Application.DataTransfareObjects.Responses;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common.Results;
using Domain.Common.Search;
using Domain.Entities;
using Domain.Enums;
using EntityFrameworkPaginateCore;

namespace Application.Services
{
    public class UserLockClaimService : IUserLockClaimService
    {
        readonly ICurrentUserService _currentUserService;
        readonly IReadRepository<UserLockClaimEntity> _mainRepo;
        readonly IReadRepository<LockAccessHistoryEntity> _accessHistoryRepo;
        readonly IMapper _mapper;

        public UserLockClaimService(ICurrentUserService currentUserService,
            IReadRepository<UserLockClaimEntity> mainRepo, IReadRepository<LockAccessHistoryEntity> accessHistoryRepo, IMapper mapper)
        {
            _currentUserService = currentUserService;
            _mainRepo = mainRepo;
            _accessHistoryRepo = accessHistoryRepo;
            _mapper = mapper;
        }
        public async Task<DataList<UserLocksResponseDto>> UserDookLockListAsync(BaseSearchQuery<UserDoorLocksSearch, GeneralSortEnum> query)
        {
            var filters = new Filters<UserLockClaimEntity>();
            if (query.Search is null) query.Search = new UserDoorLocksSearch();

            bool nameSearchHasValue = !string.IsNullOrEmpty(query.Search.Name);
            filters.Add(nameSearchHasValue, x => x.DoorLock.DoorName.Contains(query.Search.Name));

            filters.Add(true, x => x.RecordStatus != RecordStatusEnum.Deleted);
            filters.Add(true, x => x.UserId == _currentUserService.UserId);

            var sorts = new Sorts<UserLockClaimEntity>();
            sorts.Add(query.Sort == GeneralSortEnum.Id, x => x.Id, query.Order == GeneralOrderEnum.Desc);
            sorts.Add(query.Sort == GeneralSortEnum.Date, x => x.ModifiedAt, query.Order == GeneralOrderEnum.Desc);
            var page = await _mainRepo.PaginateAsync(query.CurrentPage, query.PageSize, sorts, filters, includes: "DoorLock.Floor.Building");
            return new DataList<UserLocksResponseDto>()
            {
                Results = _mapper.Map<IEnumerable<UserLocksResponseDto>>(page.Results),
                PageSize = page.PageSize,
                PageCount = page.PageCount,
                CurrentPage = page.CurrentPage,
                Total = page.RecordCount
            };
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

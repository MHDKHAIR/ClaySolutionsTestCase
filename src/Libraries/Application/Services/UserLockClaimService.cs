using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

namespace Application.Services
{
    public class UserLockClaimService : IUserLockClaimService
    {
        readonly ICurrentUserService _currentUserService;
        readonly IEfRepository<UserLockClaimEntity> _mainRepo;
        readonly IMapper _mapper;

        public UserLockClaimService(ICurrentUserService currentUserService,
            IEfRepository<UserLockClaimEntity> mainRepo, IMapper mapper)
        {
            _currentUserService = currentUserService;
            _mainRepo = mainRepo;
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

    }
}

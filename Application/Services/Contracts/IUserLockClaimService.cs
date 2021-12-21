﻿using System.Threading.Tasks;
using Application.DataTransfareObjects.Responses;
using Domain.Common.Results;
using Domain.Common.Search;
using Domain.Enums;

namespace Application.Services.Contracts
{
    public interface IUserLockClaimService
    {
        Task<DataList<UserLocksResponseDto>> UserDookLockListAsync(BaseSearchQuery<UserDoorLocksSearch, GeneralSortEnum> query);
        Task<DataList<UserAccessHistortLocksResponseDto>> UserDookLockHistoryListAsync(BaseSearchQuery<UserAccessLocksHistorySearch, GeneralSortEnum> query);
    }
}

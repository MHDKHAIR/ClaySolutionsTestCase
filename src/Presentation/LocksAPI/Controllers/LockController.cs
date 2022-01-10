using System.Threading.Tasks;
using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Results;
using Application.Common.Search;
using Application.DataTransfareObjects.Requests;
using Application.DataTransfareObjects.Responses;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocksAPI.Controllers
{
    public class LockController : BaseController
    {
        readonly IUserLockClaimService _userLockClaimService;
        readonly ILockAccessService _lockAccessService;
        readonly ILockAccessHistoryService _lockAccessHistoryService;

        public LockController(ICurrentUserService currentUser,
            IUserLockClaimService userLockClaimService,
            ILockAccessService lockAccessService, ILockAccessHistoryService lockAccessHistoryService) : base(currentUser)
        {
            _userLockClaimService = userLockClaimService;
            _lockAccessService = lockAccessService;
            _lockAccessHistoryService = lockAccessHistoryService;
        }

        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(IResult))]
        public async Task<IActionResult> LocksList(BaseSearchQuery<UserDoorLocksSearch, GeneralSortEnum> query)
        {
            var result = await _userLockClaimService.UserDookLockListAsync(query);
            return Ok(Result<DataList<UserLocksResponseDto>>.Success(result));
        }


        [HttpGet("access_history")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(IResult))]
        public async Task<IActionResult> HistoryList(BaseSearchQuery<UserAccessLocksHistorySearch, GeneralSortEnum> query)
        {
            var result = await _lockAccessHistoryService.UserDookLockHistoryListAsync(query);
            return Ok(Result<DataList<UserAccessHistortLocksResponseDto>>.Success(result));
        }

        [HttpPost("lock_access")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(IResult))]
        public async Task<IActionResult> AccessLock(AccessLockRequestDto AccessDto)
        {
            var result = await _lockAccessService.AccessLock(AccessDto);

            return Ok(Result<AccessLockResponseDto>.Success(result, "Access Granted"));
        }
    }
}

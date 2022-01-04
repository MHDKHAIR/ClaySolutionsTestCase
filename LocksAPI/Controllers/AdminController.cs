using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using Domain.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocksAPI.Controllers
{
    /// <summary>
    /// This controller for demo purposes
    /// </summary>
    public class AdminController : BaseController
    {
        readonly IUserService _userService;
        private readonly ILockAccessService _lockAccessService;

        public AdminController(ICurrentUserService currentUser, IUserService userService, ILockAccessService lockAccessService) : base(currentUser)
        {
            _userService = userService;
            _lockAccessService = lockAccessService;
        }

        [HttpGet("account_activation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(IResult))]
        public async Task<IActionResult> ActivateAccount([FromQuery] string UserId)
        {
            if (currentUser.UserType is Domain.Enums.UserTypeEnum.Admin)
            {
                await _userService.ActivateAccountAsync(UserId);
                return Ok(Result.Success("Activated"));
            }
            return Unauthorized(Result.Fail("This is only for admin"));
        }

        [HttpGet("grant_access")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(IResult))]
        public async Task<IActionResult> GrantAccessOnLock(string ClaimId)
        {
            if (currentUser.UserType is Domain.Enums.UserTypeEnum.Admin)
            {
                await _lockAccessService.GrantAccessOnLock(ClaimId);
                return Ok(Result.Success("Access graned"));
            }
            return Unauthorized(Result.Fail("This is only for admin"));

        }
    }
}

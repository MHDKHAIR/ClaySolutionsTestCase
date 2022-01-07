using System.Threading.Tasks;
using Application.Common.Results;
using Application.Services.Interfaces;
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

        public AdminController(IUserService userService, ILockAccessService lockAccessService)
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
            await _userService.ActivateAccountAsync(UserId);
            return Ok(Result.Success("Activated"));
        }

        [HttpGet("grant_access")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(IResult))]
        public async Task<IActionResult> GrantAccessOnLock(string ClaimId)
        {
            await _lockAccessService.GrantAccessOnLock(ClaimId);
            return Ok(Result.Success("Access graned"));
        }
    }
}

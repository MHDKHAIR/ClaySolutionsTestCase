using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Common.Results;
using Application.DataTransfareObjects.Requests;
using System.Threading.Tasks;
using Application.DataTransfareObjects.Responses;
using Application.Services.Interfaces;

namespace LocksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("signing_in")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(IResult))]
        public async Task<IActionResult> SigningIn(SignInRequestDto SignInDto)
        {
            var result = await _userService.AuthenticateAsync(SignInDto);
            Response.StatusCode = StatusCodes.Status201Created;
            return new JsonResult(Result<SignInResponseDto>.Success(result));
        }

        [HttpPost("registration")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        public async Task<IActionResult> Registration(RegisterUserRequestDto RegistrationDto)
        {
            var result = await _userService.RegisterUserAsync(RegistrationDto);
            Response.StatusCode = StatusCodes.Status201Created;
            return new JsonResult(result);
        }

        [HttpGet("confirmation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(IResult))]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string Id, string ConfirmationToken)
        {
            await _userService.EmailConfirmationAsync(Id, ConfirmationToken);
            return Ok(Result.Success("Email confirmed"));
        }

        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(IResult))]
        public async Task<IActionResult> AccountDelete(string UserId)
        {
            await _userService.DeleteAccountAsync(UserId);
            return Ok(Result.Success("Account id deleted"));
        }

    }
}

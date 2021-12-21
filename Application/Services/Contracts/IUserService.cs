using System.Threading.Tasks;
using Application.DataTransfareObjects.Requests;
using Application.DataTransfareObjects.Responses;
using Domain.Entities;

namespace Application.Services.Contracts
{
    public interface IUserService
    {
        Task<UserEntity> GetByIdAsync(string id, params string[] include);
        Task<SignInResponseDto> AuthenticateAsync(SignInRequestDto dto);
        Task<RegisterUserResponseDto> RegisterUserAsync(RegisterUserRequestDto dto);
        Task EmailConfirmationAsync(string userId, string token);
        Task ActivateAccountAsync(string userId);
        Task DeleteAccountAsync(string userId);
    }
}

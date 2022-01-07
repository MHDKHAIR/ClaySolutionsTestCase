using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Utils
{
    public interface IJwtUtils
    {
        string GenerateJwtToken(UserEntity user);
        JwtSecurityToken ValidateJwtToken(string token);
    }
}

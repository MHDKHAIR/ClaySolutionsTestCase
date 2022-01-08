using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Utils
{
    public interface IJwtUtils
    {
        string GenerateJwtToken(UserEntity user, bool longToken);
        JwtSecurityToken ValidateJwtToken(string token);
    }
}

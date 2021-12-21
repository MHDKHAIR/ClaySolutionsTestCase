using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Domain.Interfaces.Services;
using Domain.Enums;

namespace Application.Handlers
{
    public class JWTMiddleware
    {
        readonly RequestDelegate _next;

        public JWTMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context,
            IConfiguration configuration, ILogger<JWTMiddleware> logger,
            ICurrentUserService currentUser)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                logger.LogInformation($"Request with token: {token}");
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();

                    var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JwtSecret"));
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var userId = jwtToken.Claims.First(x => x.Type == "id").Value;
                    var userType = jwtToken.Claims.First(x => x.Type == "usertype").Value;

                    // attach user to context on successful jwt validation
                    currentUser.UserId = userId;
                    currentUser.UserType = Enum.Parse<UserTypeEnum>(userType);
                }
                catch (KeyNotFoundException)
                {
                    throw;
                }
                catch
                {
                    throw new UnauthorizedAccessException();
                    // do nothing if jwt validation fails
                    // user is not attached to context so request won't have access to secure routes
                }
            }
            await _next(context);
        }
    }
}

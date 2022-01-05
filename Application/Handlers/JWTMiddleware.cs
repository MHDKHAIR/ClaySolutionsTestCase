﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Domain.Enums;
using Application.Utils;
using Application.Common.Interfaces;
using Application.Extentions;

namespace Application.Handlers
{
    public class JWTMiddleware
    {
        readonly RequestDelegate _next;

        public JWTMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IJwtUtils jwtUtils,
            ILogger<JWTMiddleware> logger,
            ICurrentUserService currentUser)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token is not null)
            {
                var clientIp = context.GetIpAddress();
                logger.LogInformation($"Request with token: {token}\nIP Address: {clientIp}");

                var jwt = jwtUtils.ValidateJwtToken(token);
                var userId = jwt.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
                var userType = jwt.Claims.FirstOrDefault(x => x.Type == "usertype")?.Value;

                // attach user to context on successful jwt validation
                currentUser.UserId = userId;
                currentUser.UserType = Enum.Parse<UserTypeEnum>(userType);
            }
            await _next(context);
        }
    }
}

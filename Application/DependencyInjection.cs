using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using Application.Utils;
using System.Reflection;
using Application.Services.Interfaces;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserLockClaimService, UserLockClaimService>();
            services.AddScoped<ILockAccessHistoryService, LockAccessHistoryService>();
            services.AddScoped<ILockAccessService, LockAccessService>();

            return services;
        }
    }
}
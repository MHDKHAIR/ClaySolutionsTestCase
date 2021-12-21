using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using Application.Services.Contracts;
using Application.Utils;
using Domain.Interfaces.Services;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IRandomService, RandomService>();
            services.AddTransient<IGeoLocationService, GeoLocationService>();

            services.AddScoped<IpAddressUtils>();
            services.AddScoped<JwtUtils>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserLockClaimService, UserLockClaimService>();
            services.AddScoped<ILockAccessService, LockAccessService>();

            return services;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Persistence;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //DB
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("ClayAccessManagmentDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b =>
                        {
                            b.EnableRetryOnFailure(3);
                            b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        }));
            }
            //Scoped
            services.AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ILockControlService, LockControlService>();
            //Transient
            services.AddTransient<IGeoLocationService, GeoLocationService>();
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IRandomService, RandomService>();
            return services;
        }
    }
}

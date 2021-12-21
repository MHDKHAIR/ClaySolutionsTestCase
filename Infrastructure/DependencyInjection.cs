using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Domain.Interfaces.Services;
using Domain.Interfaces.Contexts;
using Infrastructure.Persistence.Configuration;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
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

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILockControlService, LockControlService>();

            services.AddTransient<IDateTimeService, DateTimeService>();

            return services;
        }
    }
}

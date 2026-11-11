using DonationHub.Application.Common.Interfaces;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Interceptors;
using DonationHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace DonationHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Register TimeProvider as a singleton service to provide the current system time throughout the application.
            services.AddSingleton<TimeProvider>(TimeProvider.System);

            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            //===============> Register the AppDbContext with the specified connection string and interceptor:
            // Register Interceptor
            services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

            // Register DbContext
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);

                options.AddInterceptors(
                    sp.GetRequiredService<ISaveChangesInterceptor>());
            });


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}

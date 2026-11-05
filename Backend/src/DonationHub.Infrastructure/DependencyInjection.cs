using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace DonationHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Add infrastructure services here, e.g. database context, repositories, etc.
            return services;
        }
    }
}

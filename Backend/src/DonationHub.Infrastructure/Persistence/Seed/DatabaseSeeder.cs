using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DonationHub.Infrastructure.Persistence.Seed;

public sealed class DatabaseSeeder(
    IHostEnvironment environment,
    InitialDataSeeder initialDataSeeder,
    DevelopmentSeeder developmentSeeder,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Database seeding started for environment {EnvironmentName}.",
            environment.EnvironmentName);

        await initialDataSeeder.SeedAsync(cancellationToken);

        if (environment.IsDevelopment())
        {
            await developmentSeeder.SeedAsync(cancellationToken);
        }

        logger.LogInformation("Database seeding completed successfully.");
    }
}

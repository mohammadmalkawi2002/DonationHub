using DonationHub.Application.Features.SecurityManager.Authorization;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Persistence.Seed;

public sealed class InitialDataSeeder(
    AppDbContext dbContext,
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<ApplicationUser> userManager,
    IOptions<SeedAdminOptions> seedAdminOptions,
    TimeProvider timeProvider,
    ILogger<InitialDataSeeder> logger)
{
    private static readonly string[] RequiredRoles =
    [
        RoleNames.Admin,
        RoleNames.CampaignManager,
        RoleNames.Donor
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Initial operational data seeding started.");

        await SeedRolesAsync();
        await SeedAdminAsync(cancellationToken);

        logger.LogInformation("Initial operational data seeding completed successfully.");
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in RequiredRoles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var role = new IdentityRole<Guid>(roleName)
            {
                Id = Guid.CreateVersion7()
            };

            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                LogIdentityFailure("create role", roleName, result);
                throw CreateIdentityOperationException("create role", roleName, result);
            }

            logger.LogInformation("Created role {RoleName}.", roleName);
        }
    }

    private async Task SeedAdminAsync(CancellationToken cancellationToken)
    {
        var options = seedAdminOptions.Value;
        var admin = await userManager.FindByEmailAsync(options.Email.Trim());

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                Id = Guid.CreateVersion7(),
                Email = options.Email.Trim(),
                UserName = options.Email.Trim(),
                FullName = options.FullName.Trim(),
                PreferredLanguage = "en",
                IsActive = true,
                CreatedAtUtc = timeProvider.GetUtcNow()
            };

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var createResult = await userManager.CreateAsync(admin, options.Password);
            if (!createResult.Succeeded)
            {
                LogIdentityFailure("create initial Admin", admin.Id.ToString(), createResult);
                throw CreateIdentityOperationException("create initial Admin", admin.Id.ToString(), createResult);
            }

            var addRoleResult = await userManager.AddToRoleAsync(admin, RoleNames.Admin);
            if (!addRoleResult.Succeeded)
            {
                LogIdentityFailure("assign Admin role", admin.Id.ToString(), addRoleResult);
                throw CreateIdentityOperationException("assign Admin role", admin.Id.ToString(), addRoleResult);
            }

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Created initial Admin account and assigned the Admin role. UserId: {UserId}.", admin.Id);
            return;
        }

        if (await userManager.IsInRoleAsync(admin, RoleNames.Admin))
        {
            return;
        }

        var roleResult = await userManager.AddToRoleAsync(admin, RoleNames.Admin);
        if (!roleResult.Succeeded)
        {
            LogIdentityFailure("assign Admin role", admin.Id.ToString(), roleResult);
            throw CreateIdentityOperationException("assign Admin role", admin.Id.ToString(), roleResult);
        }

        logger.LogInformation("Assigned the missing Admin role to the existing account. UserId: {UserId}.", admin.Id);
    }

    private void LogIdentityFailure(string operation, string subject, IdentityResult result)
    {
        logger.LogError(
            "Identity operation {Operation} failed for {Subject}. ErrorCount: {ErrorCount}, Errors: {Errors}.",
            operation,
            subject,
            result.Errors.Count(),
            FormatIdentityErrors(result));
    }

    private static InvalidOperationException CreateIdentityOperationException(
        string operation,
        string subject,
        IdentityResult result) =>
        new($"Failed to {operation} for {subject}. Identity errors: {FormatIdentityErrors(result)}");

    private static string FormatIdentityErrors(IdentityResult result) =>
        string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
}

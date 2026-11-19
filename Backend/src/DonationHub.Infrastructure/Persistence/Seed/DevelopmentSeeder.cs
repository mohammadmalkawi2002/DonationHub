using DonationHub.Application.Features.SecurityManager.Authorization;
using DonationHub.Domain.Entities.Categories;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Persistence.Seed;

public sealed class DevelopmentSeeder(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IOptions<DevelopmentSeedOptions> developmentSeedOptions,
    TimeProvider timeProvider,
    ILogger<DevelopmentSeeder> logger)
{
    private static readonly (string NameEn, string NameAr)[] DevelopmentCategories =
    [
        ("Health", "الصحة"),
        ("Education", "التعليم"),
        ("Relief", "الإغاثة"),
        ("Orphans", "الأيتام"),
        ("Environment", "البيئة")
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Development sample data seeding started.");

        var options = developmentSeedOptions.Value;
        await SeedUserAsync(options.CampaignManager, RoleNames.CampaignManager, cancellationToken);
        await SeedUserAsync(options.Donor, RoleNames.Donor, cancellationToken);
        await SeedCategoriesAsync(cancellationToken);

        logger.LogInformation("Development sample data seeding completed successfully.");
    }

    private async Task SeedUserAsync(
        DevelopmentSeedUserOptions options,
        string roleName,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(options.Email.Trim());

        if (user is null)
        {
            user = new ApplicationUser
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

            var createResult = await userManager.CreateAsync(user, options.Password);
            if (!createResult.Succeeded)
            {
                LogIdentityFailure("create Development test user", user.Id, roleName, createResult);
                throw CreateIdentityOperationException("create Development test user", user.Id, roleName, createResult);
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!addRoleResult.Succeeded)
            {
                LogIdentityFailure("assign role to Development test user", user.Id, roleName, addRoleResult);
                throw CreateIdentityOperationException("assign role to Development test user", user.Id, roleName, addRoleResult);
            }

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Created Development test user and assigned role {RoleName}. UserId: {UserId}.",
                roleName,
                user.Id);
            return;
        }

        if (await userManager.IsInRoleAsync(user, roleName))
        {
            return;
        }

        var roleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            LogIdentityFailure("assign role to Development test user", user.Id, roleName, roleResult);
            throw CreateIdentityOperationException("assign role to Development test user", user.Id, roleName, roleResult);
        }

        logger.LogInformation(
            "Assigned missing role {RoleName} to existing Development test user. UserId: {UserId}.",
            roleName,
            user.Id);
    }

   
    private async Task SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        foreach (var (nameEn, nameAr) in DevelopmentCategories)
        {
            if (await dbContext.Categories.AnyAsync(
                    category => category.NameEn == nameEn,
                    cancellationToken))
            {
                continue;
            }

            var categoryResult = Category.Create(
                nameAr,
                nameEn,
                null,
                null,
                timeProvider.GetUtcNow());

            if (categoryResult.IsError)
            {
                var errors = string.Join(
                    "; ",
                    categoryResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

                logger.LogError(
                    "Failed to create Development category {CategoryName}. Errors: {Errors}.",
                    nameEn,
                    errors);

                throw new InvalidOperationException(
                    $"Failed to create Development category '{nameEn}'. Errors: {errors}");
            }

            await dbContext.Categories.AddAsync(categoryResult.Value, cancellationToken);
            logger.LogInformation("Added Development category {CategoryName}.", nameEn);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void LogIdentityFailure(
        string operation,
        Guid userId,
        string roleName,
        IdentityResult result)
    {
        logger.LogError(
            "Identity operation {Operation} failed for Development user {UserId} and role {RoleName}. ErrorCount: {ErrorCount}, Errors: {Errors}.",
            operation,
            userId,
            roleName,
            result.Errors.Count(),
            FormatIdentityErrors(result));
    }

    private static InvalidOperationException CreateIdentityOperationException(
        string operation,
        Guid userId,
        string roleName,
        IdentityResult result) =>
        new($"Failed to {operation} with role {roleName} for Development user {userId}. Identity errors: {FormatIdentityErrors(result)}");

    private static string FormatIdentityErrors(IdentityResult result) =>
        string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
}

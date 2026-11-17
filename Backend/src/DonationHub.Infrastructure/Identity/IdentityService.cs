using DonationHub.Application.Common.ApplicationErrors;
using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Application.Features.SecurityManager.Authorization;
using DonationHub.Domain.Common.Results;
using DonationHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Identity
{
    public sealed class IdentityService(
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext,
        TimeProvider timeProvider,
        ILogger<IdentityService> logger)
        : IIdentityService
    {
        public async Task<Result<UserProfile>> CreateUserAsync(string fullName, string? preferredLanguage, string email, string password, CancellationToken ct = default)
        {
            var normalizedEmail = email.Trim();
            var user = new ApplicationUser 
            { 
                 Id=Guid.CreateVersion7(),
                Email = normalizedEmail,
                UserName = normalizedEmail,
                FullName = fullName.Trim(),
                PreferredLanguage = preferredLanguage ?? "en",
                CreatedAtUtc = timeProvider.GetUtcNow(),
                IsActive = true
            };

            await using var transaction =
        await dbContext.Database.BeginTransactionAsync(ct);

            var createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync(ct);

                var errors = MapIdentityErrors(createResult.Errors);

                logger.LogWarning(
                    "Identity failed to create user. ErrorCount: {ErrorCount}, ErrorCodes: {ErrorCodes}",
                    createResult.Errors.Count(),
                    createResult.Errors.Select(x => x.Code).ToArray());

                return Result<UserProfile>.Failure(errors);
            }

            var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Donor);

            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(ct);

                logger.LogError(
                    "Failed to assign Donor role. UserId: {UserId}, ErrorCodes: {ErrorCodes}",
                    user.Id,
                    roleResult.Errors.Select(x => x.Code).ToArray());

                return Result<UserProfile>.Failure(
                    Error.Unexpected(
                        "Auth.RoleAssignmentFailed",
                        "Unable to assign the donor role."));
            }

            await transaction.CommitAsync(ct);

            var profile = new UserProfile(
                user.Id,
                user.Email!,
                user.FullName,
                user.PreferredLanguage,
                user.CreatedAtUtc,
                [RoleNames.Donor]);

            logger.LogInformation(
                "User {UserId} registered successfully",
                user.Id);

            return Result<UserProfile>.Success(profile);

        }


        public async Task<Result<UserProfile>> CreateStaffAsync(string fullName, string email, string password, string role, CancellationToken ct = default)
        {
            var normalizedEmail = email.Trim();
            var user = new ApplicationUser
            {
                Id = Guid.CreateVersion7(),
                Email = normalizedEmail,
                UserName = normalizedEmail,
                FullName = fullName.Trim(),
                CreatedAtUtc = timeProvider.GetUtcNow(),
                IsActive = true
            };

            await using var transaction =
      await dbContext.Database.BeginTransactionAsync(ct);

            // Create the staff user and store the password hash.
            var createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync(ct);

                var errors = MapIdentityErrors(createResult.Errors);

                logger.LogWarning(
                    "Identity failed to create staff user. ErrorCount: {ErrorCount}, ErrorCodes: {ErrorCodes}",
                    createResult.Errors.Count(),
                    createResult.Errors.Select(x => x.Code).ToArray());

                return Result<UserProfile>.Failure(errors);
            }

            // Assign the requested staff role.
            var roleResult = await userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(ct);

                logger.LogError(
                    "Failed to assign staff role. UserId: {UserId}, Role: {Role}, ErrorCodes: {ErrorCodes}",
                    user.Id,
                    role,
                    roleResult.Errors.Select(x => x.Code).ToArray());

                return Result<UserProfile>.Failure(
                    Error.Unexpected(
                        "Auth.RoleAssignmentFailed",
                        "Unable to assign the staff role."));
            }

            await transaction.CommitAsync(ct);

            var profile = new UserProfile(
                user.Id,
                user.Email!,
                user.FullName,
                user.PreferredLanguage,
                user.CreatedAtUtc,
                [role]);

            logger.LogInformation(
                "Staff user {UserId} created successfully with role {Role}",
                user.Id,
                role);

            return Result<UserProfile>.Success(profile);

        }
        public async Task<Result<IdentityUserDto>> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email.Trim());

            if (user is null || !await userManager.CheckPasswordAsync(user, password))
                return Result<IdentityUserDto>.Failure(AuthErrors.InvalidCredentials);

            var roles = await userManager.GetRolesAsync(user);

            var userDto = new IdentityUserDto(
                user.Id,
                user.Email!,
                user.FullName,
                user.PreferredLanguage,
                user.CreatedAtUtc,
                user.IsActive,
                roles.ToArray());

            return Result<IdentityUserDto>.Success(userDto);
        }


        private static IReadOnlyList<Error> MapIdentityErrors(
            IEnumerable<IdentityError> identityErrors)
        {
            return identityErrors
                .Select(error =>
                    error.Code is "DuplicateEmail" or "DuplicateUserName"
                        ? AuthErrors.EmailAlreadyExists
                        : Error.Validation(
                            $"Identity.{error.Code}",
                            error.Description))
                .Distinct()
                .ToArray();
        }

        public async Task<Result<IdentityUserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Result<IdentityUserDto>.Failure(
          AuthErrors.NotFound);

            var roles = await userManager.GetRolesAsync(user);

            var userDto = new IdentityUserDto(
                user.Id,
                user.Email!,
                user.FullName,
                user.PreferredLanguage,
                user.CreatedAtUtc,
                user.IsActive,
                roles.ToArray());

            return Result<IdentityUserDto>.Success(userDto);

        }

        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Result.Failure(AuthErrors.NotFound);

            if (!user.IsActive)
                return Result.Failure(AuthErrors.AccountDeactivated);

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                var errors = MapIdentityErrors(result.Errors);

                logger.LogWarning(
                    "Password change failed for user {UserId}. ErrorCount: {ErrorCount}",
                    user.Id,
                    result.Errors.Count());

                return Result.Failure(errors);
            }

            logger.LogInformation(
                "Password changed successfully for user {UserId}",
                user.Id);

            return Result.Success();
        }
    }
}

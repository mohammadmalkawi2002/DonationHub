using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        /// <summary>
        /// Find user by email,checkpassword,GetRoles,return user data
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result<IdentityUserDto>> ValidateCredentialsAsync(
            string email,
            string password,
            CancellationToken ct = default);

        Task<Result<UserProfile>> CreateUserAsync(
            string fullName,
            string? preferredLanguage,
            string email,
            string password,
            CancellationToken ct = default);

        Task<Result<UserProfile>> CreateStaffAsync(
            string fullName,
            string email,
            string password,
            string role,
            CancellationToken ct = default);

        Task<Result<IdentityUserDto>> GetUserByIdAsync(
            Guid userId,
            CancellationToken ct = default);

        Task<Result> ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken = default);

    }
}

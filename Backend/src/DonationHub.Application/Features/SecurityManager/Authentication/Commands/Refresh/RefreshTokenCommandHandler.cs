using DonationHub.Application.Common.ApplicationErrors;
using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Refresh
{
    public sealed class RefreshTokenCommandHandler (
        IIdentityService identityService,
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,TimeProvider timeProvider)
        : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Hash incoming refresh token
            var tokenHash = tokenService.HashToken(request.RefreshToken);

            // 2. Find existing refresh token
            var refreshToken = await refreshTokenRepository.FirstOrDefaultAsync(
                      x => x.TokenHash == tokenHash,
                      cancellationToken);

            if (refreshToken is null)
                return Result<AuthResponse>.Failure(
                    AuthErrors.RefreshTokenInvalid);

            var now = timeProvider.GetUtcNow();

            // 3. Check old refresh token validity:
            if (!refreshToken.IsActive(now)) 
            {
                return Result<AuthResponse>.Failure(
                    AuthErrors.RefreshTokenInvalid);
            }

            // 4. Get user information
            var userResult = await identityService.GetUserByIdAsync(
                refreshToken.UserId,
                cancellationToken);


            if (userResult.IsError)
            {
                return userResult.TopError.Code == AuthErrors.NotFound.Code
                    ? Result<AuthResponse>.Failure(AuthErrors.RefreshTokenInvalid)
                    : Result<AuthResponse>.Failure(userResult.Errors);
            }

            var user = userResult.Value;

            if (!user.IsActive)
            {
                return Result<AuthResponse>.Failure(
                    AuthErrors.AccountDeactivated);
            }

            // 5. Generate new tokens
            var (accessToken, accessExpiresAt) = tokenService.GenerateAccessToken(user, user.Roles);

            var (rawRefreshToken, refreshTokenHash, refreshExpiresAt) = tokenService.GenerateRefreshToken();

            // 6. Create new refresh token entity
            var newRefreshTokenResult = RefreshToken.Create(
                user.Id,
                refreshTokenHash,
                refreshExpiresAt,
                now);

            if (newRefreshTokenResult.IsError)
            {
                return Result<AuthResponse>.Failure(
                    newRefreshTokenResult.Errors);
            }

            var newRefreshToken = newRefreshTokenResult.Value;

            // 7. Revoke old token and link it to the new one
            var revokeResult = refreshToken.Revoke(now, newRefreshToken.Id);

            if (revokeResult.IsError)
            {
                return Result<AuthResponse>.Failure(
                    revokeResult.Errors);
            }

            // 8. Save both changes together
            await refreshTokenRepository.AddAsync(
                newRefreshToken,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. Return new tokens

            var response = new AuthResponse(
                accessToken,
                accessExpiresAt,
                rawRefreshToken,
                refreshExpiresAt);

            return Result<AuthResponse>.Success(response);
        }
    }
}

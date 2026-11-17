using DonationHub.Application.Common.ApplicationErrors;
using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Login
{
    public sealed class LoginCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService,
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider) 
        : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate credentials
            var identityResult = await identityService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);

            if (identityResult.IsError) 
            {
                return Result<AuthResponse>.Failure(identityResult.Errors);

            }

            var user = identityResult.Value;

            // 2. Check account Activation

            if (!user.IsActive) 
            {
                return Result<AuthResponse>.Failure(AuthErrors.AccountDeactivated);

            }

            // 3. Generate access token
            var (accessToken, accessExpiresAt)= tokenService.GenerateAccessToken(user, user.Roles);

            // 4. Generate refresh token
            var (refreshToken, tokenHash, refreshExpiresAt) = tokenService.GenerateRefreshToken();

            // 5. Create refresh token entity
            var now = timeProvider.GetUtcNow();

            var refreshTokenResult = RefreshToken.Create(
                user.Id,
                tokenHash,
                refreshExpiresAt,
                now);


            if (refreshTokenResult.IsError)
            {
                return Result<AuthResponse>.Failure(
                    refreshTokenResult.Errors);
            }

            // 6. Persist refresh token
            await refreshTokenRepository.AddAsync(
                refreshTokenResult.Value,
                cancellationToken);


            await unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Return response
            return Result<AuthResponse>.Success(
                 new AuthResponse(
                     AccessToken: accessToken,
                     AccessTokenExpiresAtUtc: accessExpiresAt,
                     RefreshToken: refreshToken,
                     RefreshTokenExpiresAtUtc: refreshExpiresAt));
        }
    }
}

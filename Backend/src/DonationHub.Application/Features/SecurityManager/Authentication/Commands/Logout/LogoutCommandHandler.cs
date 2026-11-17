using DonationHub.Application.Common.Interfaces;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Logout
{
    public sealed class LogoutCommandHandler(
        ICurrentUser currentUser,
        ITokenService tokenService,
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<LogoutCommand, Result>
    {
        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            //Idempotent Logout
            var userId = currentUser.UserId;

            if (!currentUser.IsAuthenticated || userId is null)
            {
                return Result.Failure(
                    Error.Unauthorized(
                        "Auth.Unauthorized",
                        "Authentication is required."));
            }

            var tokenHash = tokenService.HashToken(request.RefreshToken);

            var refreshToken = await refreshTokenRepository.FirstOrDefaultAsync(
        x => x.TokenHash == tokenHash && x.UserId == userId.Value,
        cancellationToken);

            if (refreshToken is null)
                return Result.Success();

            var now = timeProvider.GetUtcNow();

            //IsActive()=> check not revoked and not expired:
            if (!refreshToken.IsActive(now))
                return Result.Success();

            var revokeResult = refreshToken.Revoke(now);

            if (revokeResult.IsError)
                return revokeResult;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}

using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Queries.Me
{
    public sealed class GetCurrentUserQueryHandler(ICurrentUser currentUser, IIdentityService identityService)
        : IRequestHandler<GetCurrentUserQuery, Result<UserProfile>>
    {
        public async Task<Result<UserProfile>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            if(!currentUser.IsAuthenticated || currentUser.UserId is null) 
            {
                return Result<UserProfile>.Failure(Error.Unauthorized(
                    "Auth.Unauthorized",
                    "Authentication is required."));
            }


            var userResult = await identityService.GetUserByIdAsync(
                currentUser.UserId.Value,
                cancellationToken);

            if (userResult.IsError)
                return Result<UserProfile>.Failure(userResult.Errors);

            var user = userResult.Value;

            var profile = new UserProfile(
                user.Id,
                user.Email,
                user.FullName,
                user.PreferredLanguage,
                user.CreatedAtUtc,
                user.Roles.ToList());

            return Result<UserProfile>.Success(profile);

        }
    }
}

using DonationHub.Application.Common.Interfaces;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler (IIdentityService identityService,ICurrentUser currentUser)
        : IRequestHandler<ChangePasswordCommand, Result>
    {
        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            {
                return Result.Failure(
                        Error.Unauthorized(
                            "Auth.Unauthorized",
                            "Authentication is required."));

            }

         return await  identityService.ChangePasswordAsync(
         currentUser.UserId.Value,
         request.CurrentPassword,
         request.NewPassword,
         cancellationToken);
        }
    }
}

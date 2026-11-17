using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Register
{
    public sealed class RegisterCommandHandler (IIdentityService identityService)
        : IRequestHandler<RegisterCommand, Result<UserProfile>>
    {
        public async Task<Result<UserProfile>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
           return await identityService.CreateUserAsync(
            request.FullName,
            request.PreferredLanguage,
            request.Email,
            request.Password,
            cancellationToken);

        }
    }
}

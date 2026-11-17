using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Register
{
    public sealed record RegisterCommand(
        string FullName,
        string? PreferredLanguage,
        string Email,
        string Password,
        string ConfirmPassword) : IRequest<Result<UserProfile>>;

}

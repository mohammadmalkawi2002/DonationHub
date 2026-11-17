using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Refresh
{
    public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;

}

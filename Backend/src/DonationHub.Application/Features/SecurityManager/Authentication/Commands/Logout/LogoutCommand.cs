using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Logout
{
    public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;

}

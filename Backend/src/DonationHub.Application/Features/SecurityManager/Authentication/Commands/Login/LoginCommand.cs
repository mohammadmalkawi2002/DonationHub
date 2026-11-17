using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Login
{
    public sealed record LoginCommand(string Email, string Password) 
        : IRequest<Result<AuthResponse>>;

}

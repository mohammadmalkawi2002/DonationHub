using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Queries.Me
{
    public sealed record GetCurrentUserQuery
     : IRequest<Result<UserProfile>>;
}

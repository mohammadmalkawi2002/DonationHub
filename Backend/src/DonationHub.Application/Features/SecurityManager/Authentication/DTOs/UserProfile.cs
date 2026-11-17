using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.DTOs
{
    public sealed record UserProfile(
        Guid UserId,
        string Email,
        string FullName,
        string PreferredLanguage,
        DateTimeOffset CreatedAtUtc,
        IReadOnlyList<string> Roles);
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.DTOs
{
    public sealed record IdentityUserDto(
     Guid Id,
     string Email,
     string FullName,
     string PreferredLanguage,
     DateTimeOffset CreatedAtUtc,
     bool IsActive,
     IReadOnlyCollection<string> Roles
 );
}

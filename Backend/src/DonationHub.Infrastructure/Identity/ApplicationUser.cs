using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Identity
{
    public sealed class ApplicationUser:IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public string PreferredLanguage { get; set; } = "en";
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAtUtc { get; set; }
    }
}

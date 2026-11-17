using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authorization
{
    /// <summary>
    /// Dfines the custom claim types used in the application 
    /// </summary>
    public static class ApplicationClaimTypes
    {
        public const string Subject = "sub";
        public const string Email = "email";
        public const string Name = "name";
        public const string Role = "role";
    }
}

using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Common.ApplicationErrors
{
    public static class AuthErrors
    {
        public static Error InvalidCredentials =>
       Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");

        public static Error NotFound =>
          Error.NotFound("Auth.UserNotFound", "User was not found.");

        public static Error RefreshTokenInvalid =>
     Error.Unauthorized("Auth.RefreshTokenInvalid", "The refresh token is invalid, expired or revoked.");

        public static Error EmailAlreadyExists =>
            Error.Conflict("Auth.EmailAlreadyExists", "An account with this email already exists.");

        public static Error AccountDeactivated =>
            Error.Forbidden("Auth.AccountDeactivated", "Deactivated user logs in or refreshes .");
    }

}

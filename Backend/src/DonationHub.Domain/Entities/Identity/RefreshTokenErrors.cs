using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities.Identity
{
    public static class RefreshTokenErrors
    {
        public static  Error UserIdRequired =>
       Error.Validation(
           "Validation.Failed",
           "User ID is required.");

        public static Error TokenHashRequired =>
            Error.Validation(
                "Validation.Failed",
                "Token hash is required.");

        public static  Error InvalidExpiration =>
            Error.Validation(
                "Validation.Failed",
                "Expiration time must be after creation time.");

        public static  Error AlreadyRevoked =>
            Error.Unauthorized(
                "Auth.RefreshTokenInvalid",
                "Refresh token is already revoked.");

        public static  Error InvalidReplacementToken =>
            Error.Validation(
                "Validation.Failed",
                "Replacement token ID cannot be empty.");
    }
}

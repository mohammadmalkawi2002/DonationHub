using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Common.Interfaces
{
    public interface ITokenService
    {
        (string AccessToken, DateTimeOffset ExpiresAtUtc) GenerateAccessToken(
            IdentityUserDto user,
            IEnumerable<string> roles);

        (string RefreshToken, string TokenHash, DateTimeOffset ExpiresAtUtc ) GenerateRefreshToken();

        string HashToken(string token);
    }
}

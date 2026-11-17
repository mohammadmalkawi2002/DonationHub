using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.DTOs
{
    

    public sealed record AuthResponse(
      string AccessToken,
      DateTimeOffset AccessTokenExpiresAtUtc,
      string RefreshToken,
      DateTimeOffset RefreshTokenExpiresAtUtc);

}

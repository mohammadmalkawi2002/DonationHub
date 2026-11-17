using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Application.Features.SecurityManager.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Infrastructure.Identity.Jwt
{
    public class TokenService(IOptions<JwtOptions> jwtOptions,TimeProvider timeProvider) :ITokenService
    {
        private readonly JwtOptions _options = jwtOptions.Value;
        public (string AccessToken, DateTimeOffset ExpiresAtUtc) GenerateAccessToken(IdentityUserDto user, IEnumerable<string> roles)
        {
            var now = timeProvider.GetUtcNow();
            var expiresAt = now.AddMinutes(_options.ExpiryMinutes);
            var claims = new List<Claim>
        {
                new(ApplicationClaimTypes.Subject,user.Id.ToString()),
                new(ApplicationClaimTypes.Email,user.Email??string.Empty),
                new(ApplicationClaimTypes.Name,user.FullName),
               new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
            claims.AddRange(roles.Select(role => new Claim(ApplicationClaimTypes.Role, role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler()
           .WriteToken(jwt);

            return (accessToken, expiresAt);
           
        }

        /// <summary>
        /// Generate plainText refresh token(string)
        /// </summary>
        /// <returns></returns>
        public (string RefreshToken,string TokenHash, DateTimeOffset ExpiresAtUtc) GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(bytes);
            var now = timeProvider.GetUtcNow();
            var expiresAt = now.AddDays(_options.RefreshTokenExpiryDays);
            return (refreshToken, HashToken(refreshToken), expiresAt);
        }

        public string HashToken(string token)=> Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    }
}

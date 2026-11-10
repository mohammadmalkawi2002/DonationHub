using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities.Identity
{
    public sealed class RefreshToken : Entity
    {
        private RefreshToken() { }

        private RefreshToken(
            Guid userId,
            string tokenHash,
            DateTimeOffset expiresAt,
            DateTimeOffset createdAtUtc)
        {
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = expiresAt;
            CreatedAtUtc = createdAtUtc;
        }

        public Guid UserId { get; private set; }

        public string TokenHash { get; private set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public DateTimeOffset? RevokedAt { get; private set; }

        public Guid? ReplacedByTokenId { get; private set; }

        public bool IsRevoked => RevokedAt.HasValue;

        public bool IsExpired(DateTimeOffset nowUtc)
            => ExpiresAt <= nowUtc;

        public static Result<RefreshToken> Create(
            Guid userId,
            string tokenHash,
            DateTimeOffset expiresAt,
            DateTimeOffset createdAtUtc)
        {
            if (userId == Guid.Empty)
                return Result.Failure<RefreshToken>(
                    RefreshTokenErrors.UserIdRequired);

            if (string.IsNullOrWhiteSpace(tokenHash))
                return Result.Failure<RefreshToken>(
                    RefreshTokenErrors.TokenHashRequired);

            var refreshToken=new RefreshToken(
                userId,
                tokenHash,
                expiresAt,
                createdAtUtc);

            return Result<RefreshToken>.Success(refreshToken);
        }

        public Result Revoke(DateTimeOffset revokedAt, Guid? replacedByTokenId = null)
        {
            if (IsRevoked)
                return Result.Failure(RefreshTokenErrors.AlreadyRevoked);

            if (replacedByTokenId == Guid.Empty)
                return Result.Failure(
                    RefreshTokenErrors.InvalidReplacementToken);

            RevokedAt = revokedAt;
            ReplacedByTokenId = replacedByTokenId;

            return Result.Success();
        }


    }
}

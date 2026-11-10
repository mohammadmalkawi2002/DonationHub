using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public sealed class IdempotencyRecord
    {
        private IdempotencyRecord() { } // EF Core

        public IdempotencyRecord(
            string key,
            Guid userId,
            string requestHash,
            int responseStatusCode,
            string responseBody,
            DateTimeOffset createdAtUtc,
            DateTimeOffset expiresAt)
        {
            Key = key;
            UserId = userId;
            RequestHash = requestHash;
            ResponseStatusCode = responseStatusCode;
            ResponseBody = responseBody;
            CreatedAtUtc = createdAtUtc;
            ExpiresAt = expiresAt;
        }

        public string Key { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public string RequestHash { get; private set; } = string.Empty;
        public int ResponseStatusCode { get; private set; }
        public string ResponseBody { get; private set; } = string.Empty;
        public DateTimeOffset CreatedAtUtc { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
    }
}

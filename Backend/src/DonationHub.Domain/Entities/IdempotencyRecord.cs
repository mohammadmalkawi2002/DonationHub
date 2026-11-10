using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public sealed class IdempotencyRecord
    {
        private IdempotencyRecord() { } // EF Core

        private IdempotencyRecord(
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

        public static Result<IdempotencyRecord> Create(
            string key,
            Guid userId,
            string requestHash,
            int responseStatusCode,
            string responseBody,
            DateTimeOffset createdAtUtc,
            DateTimeOffset expiresAt)
        {
            if(string.IsNullOrWhiteSpace(key))
            {
                return Result.Failure<IdempotencyRecord>(Error.Validation("IdempotencyRecord.KeyRequired", "Idempotency record key is required."));
            }

            if(userId == Guid.Empty)
            {
                return Result.Failure<IdempotencyRecord>(Error.Validation("IdempotencyRecord.UserIdRequired", "User ID is required."));
            }
            if(string.IsNullOrWhiteSpace(requestHash))
            {
                return Result.Failure<IdempotencyRecord>(Error.Validation("IdempotencyRecord.RequestHashRequired", "Request hash is required."));
            }
            if(string.IsNullOrWhiteSpace(responseBody))
            {
                return Result.Failure<IdempotencyRecord>(Error.Validation("IdempotencyRecord.ResponseBodyRequired", "Response body is required."));
            }

            if(expiresAt <= createdAtUtc)
            {
                return Result.Failure<IdempotencyRecord>(Error.Validation("IdempotencyRecord.ExpiresAtInvalid", "ExpiresAt must be greater than CreatedAtUtc."));
            }
            var record = new IdempotencyRecord(
                key.Trim(),
                userId,
                requestHash.Trim(),
                responseStatusCode,
                responseBody.Trim(),
                createdAtUtc,
                expiresAt);

            return Result.Success(record);
        }
    }
}

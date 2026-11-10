using System;
using System.Collections.Generic;
using System.Text;

using DonationHub.Domain.Common.Results;

namespace DonationHub.Domain.Entities
{
    public sealed class ProcessedStripeEvent
    {
        private ProcessedStripeEvent()
        {
        }

        private ProcessedStripeEvent(string eventId, string type, DateTimeOffset processedAt)
        {
            EventId = eventId;
            Type = type;
            ProcessedAt = processedAt;
        }

        public string EventId { get; private set; } = string.Empty;

        public string Type { get; private set; } = string.Empty;

        public DateTimeOffset ProcessedAt { get; private set; }

        public static Result<ProcessedStripeEvent> Create(
            string? eventId,
            string? type,
            DateTimeOffset processedAt)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                return Result.Failure<ProcessedStripeEvent>(
                    Error.Validation("ProcessedStripeEvent.EventIdRequired", "Event ID is required."));
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                return Result.Failure<ProcessedStripeEvent>(
                    Error.Validation("ProcessedStripeEvent.TypeRequired", "Event type is required."));
            }

            return Result.Success(new ProcessedStripeEvent(eventId, type, processedAt));
        }

    }
    }

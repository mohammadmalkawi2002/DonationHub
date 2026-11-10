using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities.Campaigns
{
    public static class CampaignErrors
    {
        // Rule 1: GoalAmount must be greater than zero.
        // Uses the official generic validation error code.
        public static readonly Error GoalAmountMustBePositive =
            Error.Validation(
                "Validation.Failed",
                "Campaign goal amount must be greater than zero.");

        // Campaign cannot be found.
        public static  Error NotFound => Error.NotFound("Campaign.NotFound", "Campaign was not found.");
        public static  Error NotActive => Error.Conflict("Campaign.NotActive", "Campaign is completed or expired");
        public static  Error GoalBelowRaised => Error.UnprocessableEntity(
            "Campaign.GoalBelowRaised",
            "Goal cannot be lower than the raised amount");

        public static Error EndDateInPast => Error.UnprocessableEntity("Campaign.EndDateInPast",
            "End date must be in the future");

        // EF Core optimistic concurrency conflict.
        public static Error ConcurrencyConflict => Error.Conflict("Campaign.ConcurrencyConflict", "Concurrent update detected");

        public static  Error TitleArRequired =>
    Error.Validation("Validation.Failed", "Arabic title is required.");

        public static  Error TitleEnRequired =>
            Error.Validation("Validation.Failed", "English title is required.");

        public static Error CategoryIdRequired=> Error.Validation("Validation.Failed", "Category Id is required");

    }
}

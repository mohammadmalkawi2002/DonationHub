using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities.Donations
{
    public static class DonationErrors
    {
        public static Error NotFound =>
            Error.NotFound(
                "Donation.NotFound",
                "Donation not found or not owned");

        public static Error AmountOutOfRange => Error.UnprocessableEntity(
            "Donation.AmountOutOfRange",
            "Amount outside the DonationOptions range");

        public static  Error InvalidAmountLimits =>
    Error.Validation(
        "Validation.Failed",
        "Donation amount limits are invalid.");

        public static Error InvalidState => Error.Conflict("Donation.InvalidState", "Donation already in a final state.");

        public static  Error CampaignIdRequired =>
       Error.Validation(
           "Validation.Failed",
           "Campaign ID is required.");

        public static  Error DonorUserIdRequired =>
            Error.Validation(
                "Validation.Failed",
                "Donor user ID is required.");

    }
}

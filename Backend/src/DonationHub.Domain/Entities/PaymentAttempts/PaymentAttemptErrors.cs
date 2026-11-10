using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities.PaymentAttempts
{
    public static class PaymentAttemptErrors
    {
        public static  Error NotFound =>
            Error.NotFound(
                "PaymentAttempt.NotFound",
                "Payment attempt was not found.");

        public static  Error DonationIdRequired =>
            Error.Validation(
                "Validation.Failed",
                "Donation ID is required.");

        public static  Error CheckoutSessionIdRequired =>
            Error.Validation(
                "Validation.Failed",
                "Stripe checkout session ID is required.");

        public static  Error InvalidState =>
            Error.Conflict(
                "PaymentAttempt.InvalidState",
                "Payment attempt cannot be modified in its current state.");
    }
}

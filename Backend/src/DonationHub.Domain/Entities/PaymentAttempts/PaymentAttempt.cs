using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;

namespace DonationHub.Domain.Entities.PaymentAttempts
{
    public sealed class PaymentAttempt : Entity
    {
        private PaymentAttempt()
        {
        }

        private PaymentAttempt(
            Guid donationId,
            string stripeCheckoutSessionId,
            Money amount,
            DateTimeOffset createdAtUtc)
        {
            DonationId = donationId;
            StripeCheckoutSessionId = stripeCheckoutSessionId;
            Amount = amount;
            Status = PaymentAttemptStatus.Created;
            CreatedAtUtc = createdAtUtc;
        }

        public Guid DonationId { get; private set; }

        public string StripeCheckoutSessionId { get; private set; } = string.Empty;

        public string? StripePaymentIntentId { get; private set; }

        public Money Amount { get; private set; } = null!;

        public PaymentAttemptStatus Status { get; private set; }
        public string? FailureCode { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public DateTimeOffset? UpdatedAtUtc { get; private set; }

        public static Result<PaymentAttempt> Create(
            Guid donationId,
            string stripeCheckoutSessionId,
            Money amount,
            DateTimeOffset createdAtUtc)
        {
            ArgumentNullException.ThrowIfNull(amount);

            if (donationId == Guid.Empty)
                return Result.Failure<PaymentAttempt>(
                    PaymentAttemptErrors.DonationIdRequired);

            if (string.IsNullOrWhiteSpace(stripeCheckoutSessionId))
                return Result.Failure<PaymentAttempt>(
                    PaymentAttemptErrors.CheckoutSessionIdRequired);

            var paymentAttempt = new PaymentAttempt(donationId, stripeCheckoutSessionId.Trim(), amount, createdAtUtc);

            return Result.Success(paymentAttempt);
        }




    }
}

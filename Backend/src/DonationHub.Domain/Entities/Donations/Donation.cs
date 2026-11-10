using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities.Donations
{
    public sealed class Donation : Entity
    {

        private Donation()
        {

        }
        private Donation(Guid campaignId, Guid donorUserId, Money amount, DateTimeOffset createdAtUtc)
        {
            CampaignId = campaignId;
            DonorUserId = donorUserId;
            Amount = amount;
            Status = DonationStatus.Pending;
            CreatedAtUtc = createdAtUtc;

        }

        public Guid CampaignId { get; private set; }

        public Guid DonorUserId { get; private set; }

        public Money Amount { get; private set; } = null!;

        public DonationStatus Status { get; private set; }

        public DateTimeOffset? CompletedAt { get; private set; }

        public string? FailureReason { get; private set; }

        public string? ReceiptNumber { get; private set; }

        public string? ReceiptFileName { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public static Result<Donation> Create(
            Guid campaignId,
            Guid donorUserId,
            Money amount,
            decimal minimumAmount,
            decimal maximumAmount,
            DateTimeOffset createdAtUtc
           )
        {
            ArgumentNullException.ThrowIfNull(amount);
            if (campaignId == Guid.Empty)
                return Result.Failure<Donation>(DonationErrors.CampaignIdRequired);

            if (donorUserId == Guid.Empty)
                return Result.Failure<Donation>(DonationErrors.DonorUserIdRequired);

            if (minimumAmount <= 0 || maximumAmount < minimumAmount)
                return Result.Failure<Donation>(DonationErrors.InvalidAmountLimits);

            if (amount.Amount < minimumAmount || amount.Amount > maximumAmount)
                return Result.Failure<Donation>(DonationErrors.AmountOutOfRange);

            var donation = new Donation(campaignId, donorUserId, amount, createdAtUtc);

            return Result.Success(donation);

        }


        public Result MarkAsSucceeded(
            DateTimeOffset completedAtUtc,
            string? receiptNumber = null,
            string? receiptFileName = null)
        {
            if (Status != DonationStatus.Pending)
                return Result.Failure(DonationErrors.InvalidState);

            Status = DonationStatus.Succeeded;
            CompletedAt = completedAtUtc;
            ReceiptNumber = receiptNumber;
            ReceiptFileName = receiptFileName;

            return Result.Success();
        }

        public Result MarkAsFailed(string? failureReason, DateTimeOffset completedAtUtc)
        {
            if (Status != DonationStatus.Pending)
                return Result.Failure(DonationErrors.InvalidState);

            Status = DonationStatus.Failed;
            FailureReason = failureReason;
            CompletedAt = completedAtUtc;

            return Result.Success();
        }
    }
    }

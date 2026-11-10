using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Interfaces;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;


namespace DonationHub.Domain.Entities.Campaigns
{
    public sealed class Campaign : Entity, ISoftDeletable
    {
        // Used by EF Core.
        private Campaign() 
        {
            
            
        }
        private Campaign(
            string titleAr, string titleEn, string descriptionAr, string descriptionEn, Money goalAmount,
            Money raisedAmount,
            DateTimeOffset endDate,
            Guid categoryId,
            string? bannerFileName,
            Guid createdByUserId,
            DateTimeOffset createdAtUtc)
        {
            TitleAr = titleAr;
            TitleEn = titleEn;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;

            GoalAmount = goalAmount;
            RaisedAmount = raisedAmount;

            EndDate = endDate;
            CreatedAtUtc = createdAtUtc;
            CategoryId = categoryId;
            BannerFileName = bannerFileName;

            CreatedByUserId = createdByUserId;
            Status = CampaignStatus.Active;
        }


        public string TitleAr { get; private set; } = string.Empty;

        public string TitleEn { get; private set; } = string.Empty;

        public string DescriptionAr { get; private set; } = string.Empty;

        public string DescriptionEn { get; private set; } = string.Empty;

        public Money GoalAmount { get; private set; } = null!;

        public Money RaisedAmount { get; private set; }= null!;

        public DateTimeOffset EndDate { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public DateTimeOffset? UpdatedAtUtc { get; private set; }

        public Guid CategoryId { get; private set; }

        public string? BannerFileName { get; private set; }
        public CampaignStatus Status { get; private set; }
        public Guid CreatedByUserId { get; private set; }

        public bool IsDeleted { get;  set; }

        public DateTimeOffset? DeletedAtUtc { get;  set; }
        public byte[] RowVersion { get; private set; } = [];

        
        public static Result<Campaign> Create(
            string titleAr,
            string titleEn,
            string descriptionAr,
            string descriptionEn,
            Money goalAmount,
            DateTimeOffset endDate,
            Guid categoryId,
            Guid createdByUserId,
            DateTimeOffset nowUtc,
            string? bannerFileName=null)
        {
            if (string.IsNullOrWhiteSpace(titleAr))
                return Result.Failure<Campaign>(CampaignErrors.TitleArRequired);

            if (string.IsNullOrWhiteSpace(titleEn))
                return Result.Failure<Campaign>(CampaignErrors.TitleEnRequired);

            if (categoryId == Guid.Empty)
                return Result.Failure<Campaign>(CampaignErrors.CategoryIdRequired);

            // Rule 1: GoalAmount must be greater than zero.
            if (goalAmount.Amount <= 0)
            {
                return Result.Failure<Campaign>(
                    CampaignErrors.GoalAmountMustBePositive);
            }

            // Rule 2: EndDate must be in the future.
            if (endDate <= nowUtc)
            {
                return Result.Failure<Campaign>(
                    CampaignErrors.EndDateInPast);
            }

            // A new campaign starts with zero raised amount.
            var raisedAmount = Money.Zero(goalAmount);
            var campaign = new Campaign(
                titleAr,
                titleEn,
                descriptionAr,
                descriptionEn,
                goalAmount,
                raisedAmount,
                endDate,
                categoryId,
                bannerFileName,
                createdByUserId,
                nowUtc);

            return Result.Success(campaign);
        }

        /// <summary>
        ///Changes the campaign end date after validating the new date.
        /// </summary>
        /// <param name="newEndDate"></param>
        /// <param name="nowUtc"></param>
        /// <returns></returns>
        public Result ChangeEndDate(DateTimeOffset newEndDate, DateTimeOffset nowUtc)
        {
            if (newEndDate <= nowUtc)
            {
                return Result.Failure(
                    CampaignErrors.EndDateInPast);
            }

            EndDate = newEndDate;
            UpdatedAtUtc = nowUtc;
            return Result.Success();
        }

        /// <summary>
        /// Changes the campaign goal without allowing it below RaisedAmount.
        /// </summary>
        /// <param name="newGoalAmount"></param>
        /// <returns></returns>
        public Result ChangeGoalAmount(Money newGoalAmount, DateTimeOffset updatedAtUtc)
        { 
            ArgumentNullException.ThrowIfNull(newGoalAmount);

            if (newGoalAmount.Amount <= 0)
            {
                return Result.Failure(
                    CampaignErrors.GoalAmountMustBePositive);
            }

            var comparisonResult = newGoalAmount.IsGreaterThanOrEqualTo(RaisedAmount);

            if (comparisonResult.IsError) 
            {
                return Result.Failure(comparisonResult.Errors);

            }

            // Rule 7: GoalAmount cannot be lower than RaisedAmount.(here mean newGoalAmount<=RaisedAmount())
            if (!comparisonResult.Value)
            {
                return Result.Failure(
                    CampaignErrors.GoalBelowRaised);
            }

            GoalAmount = newGoalAmount;
            UpdatedAtUtc = updatedAtUtc;
            return Result.Success();

        }

      

        /// <summary>
        ///  Checks whether the campaign can accept a new donation.
        /// </summary>
        /// <param name="nowUtc"></param>
        /// <returns></returns>
        public Result CanAcceptDonation(DateTimeOffset nowUtc)
        {
            // Rule 3: Campaign must be Active.
            if (Status != CampaignStatus.Active)
            {
                return Result.Failure(
                    CampaignErrors.NotActive);
            }

            // Rule 3: Campaign end date must not have passed.
            if (EndDate <= nowUtc)
            {
                return Result.Failure(
                    CampaignErrors.NotActive);
            }

            return Result.Success();
        }

        public Result RecordSuccessfulDonation(Money donationAmount, DateTimeOffset updatedAtUtc)
        {
            ArgumentNullException.ThrowIfNull(donationAmount);
            // Rule 5: Add only the amount of a successful donation:
            var additionResult = RaisedAmount.Add(donationAmount);

            if (additionResult.IsError)
            {
                return Result.Failure(additionResult.Errors);
            }

            var newRaisedAmount = additionResult.Value;
            // Rule 6: Check whether the campaign has reached its goal.
            var comparisonResult =
                newRaisedAmount.IsGreaterThanOrEqualTo(GoalAmount);

            if (comparisonResult.IsError)
            {
                return Result.Failure(comparisonResult.Errors);
            }
            
            RaisedAmount = newRaisedAmount;
            UpdatedAtUtc = updatedAtUtc;
            // If the raised amount has reached or exceeded the goal, mark the campaign as Completed.
            // Complete only if the campaign is still Active.
            if (Status == CampaignStatus.Active && comparisonResult.Value)
            {
                MarkAsCompleted();
            }

            return Result.Success();
        }


        // Expires the campaign only when its end date has passed.
        public Result<bool> TryMarkExpired(DateTimeOffset nowUtc)
        {
            // Final states cannot transition to another state.
            if (Status != CampaignStatus.Active)
            {
                return Result.Success(false);
            }

            if (EndDate > nowUtc)
            {
                return Result.Success(false);
            }

            Status = CampaignStatus.Expired;
            UpdatedAtUtc = nowUtc;

            return Result.Success(true);
        }

        /// <summary>
        /// Changes the campaign to Completed when its goal is reached.
        /// </summary>
        private void MarkAsCompleted()
        {
            Status = CampaignStatus.Completed;
        }

    }
}

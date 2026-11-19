using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;


namespace DonationHub.Domain.Entities.Categories
{
    public sealed class Category : Entity
    {
        private Category(
            string nameAr,
            string nameEn,
            string? descriptionAr,
            string ?descriptionEn,
            DateTimeOffset createdAtUtc)
        {
            NameAr = nameAr;
            NameEn = nameEn;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            CreatedAtUtc = createdAtUtc;
        }

        // Used by EF Core.
        private Category()
        {
        }
        public const int MaxNameLength = 100;

        public string NameAr { get; private set; } = string.Empty;
        public string NameEn { get; private set; } = string.Empty;
        public string? DescriptionAr { get; private set; } = string.Empty;
        public string? DescriptionEn { get; private set; } = string.Empty;
        public DateTimeOffset CreatedAtUtc { get; private set; }
        public DateTimeOffset? UpdatedAtUtc { get; private set; }

        public static Result<Category> Create(
            string? nameAr,
            string? nameEn,
            string? descriptionAr,
            string? descriptionEn,
            DateTimeOffset createdAtUtc)
        {
            var validation = ValidateNames(nameAr, nameEn);
            if (validation.IsError)
                return Result<Category>.Failure(validation.Errors);

            var category = new Category(
                nameAr!.Trim(),
                nameEn!.Trim(),
                Normalize(descriptionAr),
                Normalize(descriptionEn),
                createdAtUtc);

            return Result<Category>.Success(category);
        }

        public Result Update(
            string? nameAr,
            string? nameEn,
            string? descriptionAr,
            string? descriptionEn,
            DateTimeOffset updatedAtUtc)
        {
            var validation = ValidateNames(nameAr, nameEn);

            if (validation.IsError)
                return validation;

            NameAr = nameAr!.Trim();
            NameEn = nameEn!.Trim();
            DescriptionAr = Normalize(descriptionAr);
            DescriptionEn = Normalize(descriptionEn);
            UpdatedAtUtc = updatedAtUtc;

            return Result.Success();
        }

        private static Result ValidateNames(string? nameAr, string? nameEn)
        {
            if (string.IsNullOrWhiteSpace(nameAr)) 
                return Result.Failure(CategoryErrors.NameArRequired);
            if (nameAr.Trim().Length > MaxNameLength) return Result.Failure(CategoryErrors.NameArTooLong);
            if (string.IsNullOrWhiteSpace(nameEn)) return Result.Failure(CategoryErrors.NameEnRequired);
            if (nameEn.Trim().Length > MaxNameLength) return Result.Failure(CategoryErrors.NameEnTooLong);
            return Result.Success();
        }

        public Result CanDeleteCategory(bool hasCampaigns)
        {
            if (hasCampaigns)
                return Result.Failure(CategoryErrors.HasCampaigns);

            return Result.Success();
        }
        private static string? Normalize(string? value)
    => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

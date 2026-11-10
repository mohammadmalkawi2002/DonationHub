using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;


namespace DonationHub.Domain.Entities.Categories
{
    public sealed class Category : Entity
    {
        private Category(
            string nameAr,
            string nameEn,
            string descriptionAr,
            string descriptionEn,
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

        public string NameAr { get; private set; } = string.Empty;
        public string NameEn { get; private set; } = string.Empty;
        public string DescriptionAr { get; private set; } = string.Empty;
        public string DescriptionEn { get; private set; } = string.Empty;
        public DateTimeOffset CreatedAtUtc { get; private set; }
        public DateTimeOffset? UpdatedAtUtc { get; private set; }

        public static Result<Category> Create(
            string? nameAr,
            string? nameEn,
            string? descriptionAr,
            string? descriptionEn,
            DateTimeOffset createdAtUtc)
        {
            if (string.IsNullOrWhiteSpace(nameAr))
            {
                return Result.Failure<Category>(CategoryErrors.NameArRequired); 

            }

            if (nameAr.Trim().Length > 100) 
            { 
                return Result.Failure<Category>(CategoryErrors.NameArTooLong);
            }

                if (string.IsNullOrWhiteSpace(nameEn))
            {
                return Result.Failure<Category>(CategoryErrors.NameEnRequired);

            }

            if (nameEn.Trim().Length > 100)
            {
                return Result.Failure<Category>(CategoryErrors.NameEnTooLong);
            }

            var category = new Category(
                nameAr.Trim(),
                nameEn.Trim(),
                descriptionAr?.Trim() ?? string.Empty,
                descriptionEn?.Trim() ?? string.Empty,
                createdAtUtc);

            return Result.Success(category);
        }

        public Result Update(
            string? nameAr,
            string? nameEn,
            string? descriptionAr,
            string? descriptionEn,
            DateTimeOffset updatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(nameAr))
            {
                return Result.Failure(CategoryErrors.NameArRequired);

            }

            if (nameAr.Trim().Length > 100)
            {
                return Result.Failure(CategoryErrors.NameArTooLong);
            }

            if (string.IsNullOrWhiteSpace(nameEn))
            {
                return Result.Failure(CategoryErrors.NameEnRequired);

            }

            if (nameEn.Trim().Length > 100)
            {
                return Result.Failure(CategoryErrors.NameEnTooLong);
            }

            NameAr = nameAr!.Trim();
            NameEn = nameEn!.Trim();
            DescriptionAr = descriptionAr?.Trim() ?? string.Empty;
            DescriptionEn = descriptionEn?.Trim() ?? string.Empty;
            UpdatedAtUtc = updatedAtUtc;

            return Result.Success();
        }

    }
}

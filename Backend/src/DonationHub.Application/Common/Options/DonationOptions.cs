using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Common.Options;

public sealed class DonationOptions : IValidatableObject
{
    public const string SectionName = "Donation";

    [Range(typeof(decimal), "1.00", "10000.00")]
    public decimal MinAmount { get; set; } = 1.00m;

    [Range(typeof(decimal), "1.00", "10000.00")]
    public decimal MaxAmount { get; set; } = 10000.00m;

    [Range(1, int.MaxValue)]
    public int PendingTimeoutMinutes { get; set; } = 30;

    [Required]
    [RegularExpression("^[A-Z]{3}$")]
    public string DefaultCurrency { get; set; } = "USD";

    /// <summary>
    /// Cross-property Validation. 
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinAmount > MaxAmount)
        {
            yield return new ValidationResult("MinAmount must be less than or equal to MaxAmount.", new[] { nameof(MinAmount), nameof(MaxAmount) });
        }
    }
}

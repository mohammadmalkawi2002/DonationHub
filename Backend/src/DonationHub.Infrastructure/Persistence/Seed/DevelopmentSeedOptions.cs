using System.ComponentModel.DataAnnotations;

namespace DonationHub.Infrastructure.Persistence.Seed;

public sealed class DevelopmentSeedOptions
{
    public const string SectionName = "DevelopmentSeed";

    public DevelopmentSeedUserOptions CampaignManager { get; set; } = new();

    public DevelopmentSeedUserOptions Donor { get; set; } = new();
}

public sealed class DevelopmentSeedUserOptions
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

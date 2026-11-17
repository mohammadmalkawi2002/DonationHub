using System.ComponentModel.DataAnnotations;

namespace DonationHub.Infrastructure.Identity.Jwt;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    [Required]
    [MinLength(32)]
    public string SecretKey { get; set; } = null!;

    [Required]
    public string Issuer { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; set; }

    
    [Range(1, int.MaxValue)]
    public int RefreshTokenExpiryDays { get; set; }

}

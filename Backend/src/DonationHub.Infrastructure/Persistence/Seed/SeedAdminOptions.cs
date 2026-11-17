using System.ComponentModel.DataAnnotations;
using System.Formats.Cbor;

namespace DonationHub.Infrastructure.Persistence.Seed;

public sealed class SeedAdminOptions
{
    public const string SectionName = "SeedAdmin";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;

    [Required]
    public string FullName { get; set; } = null!;

   


    
    

}



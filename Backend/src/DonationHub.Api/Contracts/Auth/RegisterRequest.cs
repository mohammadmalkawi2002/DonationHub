namespace DonationHub.Api.Contracts.Auth
{
    public sealed record RegisterRequest(string FullName,
        string? PreferredLanguage,
        string Email,
        string Password,
        string ConfirmPassword);
    
}

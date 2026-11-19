namespace DonationHub.Api.Contracts.Admin
{
    public sealed record CreateStaffRequest(string FullName, string Email, string Role);
   
}

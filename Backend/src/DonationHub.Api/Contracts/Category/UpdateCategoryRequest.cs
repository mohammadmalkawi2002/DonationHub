namespace DonationHub.Api.Contracts.Category
{
    public sealed record class UpdateCategoryRequest(string NameAr, string NameEn, string? DescriptionAr, string? DescriptionEn);
   
}

namespace DonationHub.Api.Contracts.Category
{
    public sealed record CreateCategoryRequest(string NameAr, string NameEn, string? DescriptionAr, string? DescriptionEn);
    
}

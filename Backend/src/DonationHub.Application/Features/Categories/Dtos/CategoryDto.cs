using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Dtos
{
    public sealed record CategoryDto(
     Guid Id, string NameAr, string NameEn, string? DescriptionAr,
     string? DescriptionEn, DateTimeOffset CreatedAtUtc, DateTimeOffset? UpdatedAtUtc);
}

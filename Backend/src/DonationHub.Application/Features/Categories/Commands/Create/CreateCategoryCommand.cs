using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.Create
{
    public sealed record CreateCategoryCommand(
        string NameAr, string NameEn, string? DescriptionAr, string? DescriptionEn)
        : IRequest<Result<CategoryDto>>;
}

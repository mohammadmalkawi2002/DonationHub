using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.Update
{
    public sealed record UpdateCategoryCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn
) : IRequest<Result>;
}

using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetAll
{
    public sealed record GetCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryDto>>>;


}

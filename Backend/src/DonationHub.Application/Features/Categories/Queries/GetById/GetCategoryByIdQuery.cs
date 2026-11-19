using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetById
{
    public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;


}

using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetAll
{
    public sealed class GetCategoriesQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
    {
        public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await dbContext.Categories
                                .AsNoTracking()
                                .OrderByDescending(c=>c.CreatedAtUtc)
                                .ThenBy(c=>c.Id)
                                .Select(c => new CategoryDto(
                                    c.Id,
                                    c.NameAr,
                                    c.NameEn,
                                    c.DescriptionAr,
                                    c.DescriptionEn,
                                    c.CreatedAtUtc,
                                    c.UpdatedAtUtc))
                                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<CategoryDto>>.Success(categories);
        }
    }
}

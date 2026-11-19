using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DonationHub.Application.Features.Categories.Queries.GetById
{
    public sealed class GetCategoryByIdQueryHandler(IAppDbContext dbContext, ILogger<GetCategoryByIdQueryHandler> logger)
     : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            var category = await dbContext.Categories
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

            if(category is null) 
            {
                logger.LogWarning("Category with id {CategoryId} was not found",query.Id );

                return Result<CategoryDto>.Failure(CategoryErrors.NotFound);

            }

            var result = new CategoryDto(
                category.Id,
                category.NameAr,
                category.NameEn,
                category.DescriptionAr,
                category.DescriptionEn,
                category.CreatedAtUtc,
                category.UpdatedAtUtc);

            return Result<CategoryDto>.Success(result);

        }
    }
}

using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities.Categories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.Create
{
    public sealed class CreateCategoryCommandHandler(
        IGenericRepository<Category> categories,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        ILogger<CreateCategoryCommandHandler> logger) 
        : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
    {
        public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {

           //Create Category through the domain factory and check the result. 
         //why i call create first:Cheap checks before expensive ones. Domain validation is in-memory, so invalid input never costs a DB query.

            var now = timeProvider.GetUtcNow();
            var categoryResult = Category.Create(
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn,
                now);

            if (categoryResult.IsError)
                return Result<CategoryDto>.Failure(categoryResult.Errors);

            var category = categoryResult.Value;



            //  Check names uniqueness.

            var duplicateAr = await categories.ExistsAsync(x => x.NameAr==category.NameAr,cancellationToken);

            if (duplicateAr)
            {
                logger.LogWarning(
                    "Category creation failed. Arabic name already exists.");

                return Result<CategoryDto>.Failure(
                    CategoryErrors.NameArAlreadyExists);
            }

            var duplicateEn = await categories.ExistsAsync(x => x.NameEn == category.NameEn,cancellationToken);

            if (duplicateEn)
            {
                logger.LogWarning(
                    "Category creation failed. English name already exists.");

                return Result<CategoryDto>.Failure(
                    CategoryErrors.NameEnAlreadyExists);
            }

            await categories.AddAsync(category, cancellationToken);
           
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Category created successfully. CategoryId: {CategoryId}",
                category.Id);

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

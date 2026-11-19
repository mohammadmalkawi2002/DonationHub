using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities.Categories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.Update
{
    public sealed class UpdateCategoryCommandHandler(
        IGenericRepository<Category> categories,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        ILogger<UpdateCategoryCommandHandler> logger)
    : IRequestHandler<UpdateCategoryCommand, Result>
    {
        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            
            var category = await categories.GetByIdAsync(request.Id, cancellationToken);

            if (category is null)
            {
                logger.LogWarning("Category {CategoryId} not found for update.", request.Id);
                return Result.Failure(CategoryErrors.NotFound);

            }

            //check if Names exist in DB but excpet this cat record

            var duplicateAr = await categories.ExistsAsync(x => x.NameAr == request.NameAr.Trim() && x.Id !=request.Id, cancellationToken);

            if (duplicateAr)
            {
                logger.LogWarning(
                    "Category {CategoryId} Update failed. Arabic name already exists.", request.Id);

                return Result.Failure(
                    CategoryErrors.NameArAlreadyExists);
            }

            var duplicateEn = await categories.ExistsAsync(x => x.NameEn == request.NameEn.Trim() && x.Id!=request.Id, cancellationToken);

            if (duplicateEn)
            {
                logger.LogWarning(
                    "Category  {CategoryId} Update failed. English name already exists.",request.Id);

                return Result.Failure(
                    CategoryErrors.NameEnAlreadyExists);
            }

            //call update method

            var updateResult= category.Update(
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn,
                timeProvider.GetUtcNow());


            if (updateResult.IsError)
                return Result.Failure(updateResult.Errors);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

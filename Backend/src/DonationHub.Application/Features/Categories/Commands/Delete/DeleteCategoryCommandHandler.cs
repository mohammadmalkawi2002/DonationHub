using DonationHub.Application.Common.Interfaces;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities.Campaigns;
using DonationHub.Domain.Entities.Categories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DonationHub.Application.Features.Categories.Commands.Delete
{
    public sealed class DeleteCategoryCommandHandler(
        IGenericRepository<Category> categories,
        IGenericRepository<Campaign> campaigns,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCategoryCommandHandler> logger) 
        : IRequestHandler<DeleteCategoryCommand, Result>
    {
        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            //find the category :
            var category = await categories.GetByIdAsync(request.CategoryId, cancellationToken);
            if(category is null)
            {
                logger.LogWarning("Category with id {CategoryId} not found for deletion.", request.CategoryId);
                return Result.Failure(CategoryErrors.NotFound);
            }

            //check cat has campagins:

            var hasCampaigns = await campaigns.ExistsAsync(x => x.CategoryId == request.CategoryId, cancellationToken, ignoreQueryFilters: true);

            var canDeleteResult = category.CanDeleteCategory(hasCampaigns);


            if (canDeleteResult.IsError) 
            {
                logger.LogWarning("Category {CategoryId} cannot be deleted because it has associated campaigns.", request.CategoryId);

                return Result.Failure(canDeleteResult.Errors);
            }

            categories.Remove(category);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Category {CategoryId} deleted successfully.", request.CategoryId);
            return Result.Success();
        }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetById
{
    public sealed class GetCategoryByIdQueryValidator:AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Category Id is required");
        }
    }

}

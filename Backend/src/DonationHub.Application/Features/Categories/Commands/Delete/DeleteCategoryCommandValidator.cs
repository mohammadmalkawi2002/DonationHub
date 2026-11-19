using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.Delete
{
    public sealed class DeleteCategoryCommandValidator:AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
           .NotEmpty().WithMessage("Category Id is required.");
        }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.Update
{
    public sealed class UpdateCategoryCommandValidator
    : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Category Id is required");

            RuleFor(x => x.NameAr)
                .NotEmpty()
                .WithMessage("Category NameAr is required")
                .MaximumLength(100);

            RuleFor(x => x.NameEn)
                .NotEmpty()
                .WithMessage("Category NameEn is required")
                .MaximumLength(100);
        }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.Create
{
    public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
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

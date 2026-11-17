using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Register
{
    public sealed class RegisterCommandValidator:AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
              .NotEmpty()
             .WithMessage("Email is required")
             .EmailAddress()
             .WithMessage("Invalid email address");

            RuleFor(x => x.FullName)
              .NotEmpty()
              .WithMessage("FullName is required");

            RuleFor(x =>x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("ConfirmPassword is required")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match");

            
            RuleFor(x => x.PreferredLanguage)
                .Must(lang => lang is null || lang == "en" || lang == "ar")
                .WithMessage("Preferred language must be 'en' or 'ar'.");

        }
    }
}

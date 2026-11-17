using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using DonationHub.Domain.Common.Results;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Login
{
    public sealed class LoginCommandValidator:AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
              .NotEmpty()
              .WithMessage("Email is required")
              .EmailAddress()
              .WithMessage("Invalid email address");

            RuleFor(x => x.Password)
              .NotEmpty()
              .WithMessage("Password is required");
              
        }
    }
}

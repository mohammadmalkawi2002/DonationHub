using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.SecurityManager.Authentication.Commands.Logout
{
    public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefrehToken is required");

        }
    }

}

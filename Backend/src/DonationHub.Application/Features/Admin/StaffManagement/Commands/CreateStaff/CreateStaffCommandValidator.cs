using DonationHub.Application.Features.SecurityManager.Authorization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Admin.StaffManagement.Commands.CreateStaff
{
    public sealed class CreateStaffCommandValidator:AbstractValidator<CreateStaffCommand>
    {
        public CreateStaffCommandValidator()
        {
            RuleFor(x => x.Email)
              .NotEmpty()
             .WithMessage("Email is required")
             .EmailAddress()
             .WithMessage("Invalid email address");

            RuleFor(x => x.FullName)
              .NotEmpty()
              .WithMessage("FullName is required");

            RuleFor(x => x.Role)
           .NotEmpty()
           .WithMessage("Role is required")
           .Must(role =>
               role == RoleNames.Admin ||
               role == RoleNames.CampaignManager)
           .WithMessage("Role must be Admin or CampaignManager.");

        }
    }
}

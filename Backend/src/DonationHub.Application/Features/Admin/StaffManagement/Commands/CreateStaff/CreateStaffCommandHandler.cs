using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Admin.StaffManagement.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Admin.StaffManagement.Commands.CreateStaff
{
    public sealed class CreateStaffCommandHandler(IIdentityService identityService,ITemporaryPasswordGenerator passwordGenerator)
        : IRequestHandler<CreateStaffCommand, Result<CreatedStaff>>
    {
        public async Task<Result<CreatedStaff>> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
        {
            // Generate a temporary password for the new staff user.
            var temporaryPassword = passwordGenerator.Generate();

            // Create the staff user and assign the requested role.
            var result = await identityService.CreateStaffAsync(
                request.FullName,
                request.Email,
                temporaryPassword,
                request.Role,
                cancellationToken);

            if (result.IsError)
                return Result<CreatedStaff>.Failure(result.Errors);

            // Combine the user profile with the temporary password.
            var createdStaff = new CreatedStaff(result.Value, temporaryPassword);

            return Result<CreatedStaff>.Success(createdStaff);
        }
    }
}

using DonationHub.Application.Features.Admin.StaffManagement.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Admin.StaffManagement.Commands.CreateStaff
{
    public sealed record CreateStaffCommand(string FullName, string Email, string Role):IRequest<Result<CreatedStaff>>;
    
}

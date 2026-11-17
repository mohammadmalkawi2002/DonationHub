using DonationHub.Application.Features.SecurityManager.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Admin.StaffManagement.Dtos
{
    public sealed record CreatedStaff(UserProfile User, string TemporaryPassword);

}

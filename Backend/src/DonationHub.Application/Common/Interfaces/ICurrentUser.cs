using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }
        string? Email { get; }
        string? FullName { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);


    }
}

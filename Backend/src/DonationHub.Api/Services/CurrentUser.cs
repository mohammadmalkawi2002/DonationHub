using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.SecurityManager.Authorization;
using System.Security.Claims;

namespace DonationHub.Api.Services
{
    public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

        public Guid? UserId => Guid.TryParse(User?.FindFirstValue(ApplicationClaimTypes.Subject),out var value)?value:null;
        public string? Email => User?.FindFirstValue(ApplicationClaimTypes.Email);

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

        public string? FullName => User?.FindFirstValue(ApplicationClaimTypes.Name);

        

        public bool IsInRole(string role) => User?.IsInRole(role) == true;


    }
}

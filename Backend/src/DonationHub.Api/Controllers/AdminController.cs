using Asp.Versioning;
using DonationHub.Api.Contracts.Admin;
using DonationHub.Application.Features.Admin.StaffManagement.Commands.CreateStaff;
using DonationHub.Application.Features.SecurityManager.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    [Route("api/v{version:apiVersion}/admin")]
    [Authorize(Roles =RoleNames.Admin)]
    [ApiVersion("1.0")]
    [Tags("Admin")]
    public class AdminController(ISender sender) : ApiController
    {
        [HttpPost("staff")]
        [EndpointName("CreateStaffV1")]
        [EndpointSummary("Creates an Admin or Campaign Manager account")]
        [EndpointDescription("Creates a staff account and returns the profile with a one-time temporary password.")]
        public async Task<IActionResult> CreateStaff(CreateStaffRequest req, CancellationToken ct)
        {
            var result = await sender.Send(new CreateStaffCommand(req.FullName,req.Email,req.Role), ct);

            return result.IsError
                  ? Problem(result.Errors.ToList())
                  : StatusCode(StatusCodes.Status201Created, result.Value);
        }
    }
}

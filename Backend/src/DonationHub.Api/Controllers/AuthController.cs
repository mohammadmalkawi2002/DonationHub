using Asp.Versioning;
using DonationHub.Api.Contracts.Auth;
using DonationHub.Application.Features.SecurityManager.Authentication.Commands.ChangePassword;
using DonationHub.Application.Features.SecurityManager.Authentication.Commands.Login;
using DonationHub.Application.Features.SecurityManager.Authentication.Commands.Logout;
using DonationHub.Application.Features.SecurityManager.Authentication.Commands.Refresh;
using DonationHub.Application.Features.SecurityManager.Authentication.Commands.Register;
using DonationHub.Application.Features.SecurityManager.Authentication.Queries.Me;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    [Tags("Auths")]
    public class AuthController(ISender sender) : ApiController
    {
        [AllowAnonymous]
        [HttpPost("register")]
        [EndpointSummary("Registers a new user")]
        [EndpointDescription("Creates a new user account with the specified credentials and  default role.")]
        public async Task<IActionResult> Register([FromBody]RegisterRequest req, CancellationToken ct) 
        {
            var result = await sender.Send(
                new RegisterCommand(req.FullName, req.PreferredLanguage, req.Email, req.Password, req.ConfirmPassword),
                ct);

            return result.IsError
                    ? Problem(result.Errors.ToList())
                    : Created("", result.Value);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [EndpointName("LoginV1")]
        [EndpointSummary("Authenticates a user")]
        [EndpointDescription("Authenticates a user and returns an access token and refresh token.")]
        public async Task<IActionResult> Login(LoginRequest req, CancellationToken ct)
        {
            var result = await sender.Send(new LoginCommand(req.Email, req.Password), ct);

            return result.IsError
                   ? Problem(result.Errors.ToList())
                   : Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [EndpointName("RefreshTokenV1")]
        [EndpointSummary("Refreshes an access token")]
        [EndpointDescription("Uses a valid refresh token to issue a new access token and refresh token.")]
        public async Task<IActionResult> Refresh(RefreshRequest req, CancellationToken ct)
        {
            var result = await sender.Send(new RefreshTokenCommand(req.RefreshToken), ct);

            return result.IsError
                    ? Problem(result.Errors.ToList())
                    : Ok(result.Value);
        }


        [Authorize]
        [HttpPost("logout")]
        [EndpointName("LogoutV1")]
        [EndpointSummary("Logs out the current user")]
        [EndpointDescription("Revokes the current refresh token and completes the logout operation.")]
        public async Task<IActionResult> Logout(LogoutRequest req, CancellationToken ct)
        {
            var result = await sender.Send(new LogoutCommand(req.RefreshToken), ct);
            return result.IsError
                  ? Problem(result.Errors.ToList())
                  : NoContent();
        }


        [Authorize]
        [HttpGet("me")]
        [EndpointName("GetCurrentUserV1")]
        [EndpointSummary("Gets the current authenticated user Profile Info")]
        [EndpointDescription("Returns the profile information of the currently authenticated user.")]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var result = await sender.Send(new GetCurrentUserQuery(), ct);

            return result.IsError
                    ? Problem(result.Errors.ToList())
                    : Ok(result.Value);
        }

        [HttpPost("changePassword")]
        [Authorize]
        [EndpointName("ChangePasswordV1")]
        [EndpointSummary("Changes the current user's password")]
        [EndpointDescription("Changes the authenticated user's password after validating the current password.")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req, CancellationToken ct)
        {
            var result = await sender.Send(
                new ChangePasswordCommand(req.CurrentPassword,req.NewPassword),ct);

            return result.IsError
                   ? Problem(result.Errors.ToList())
                   : NoContent();
        }
    }
}

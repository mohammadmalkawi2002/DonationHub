using Microsoft.AspNetCore.Diagnostics;
using System.Collections;
using System.Diagnostics;

namespace DonationHub.Api.Infrastructure;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
       // Log the exception with the full stack trace.:
        logger.LogError(exception,
            "Unhandled exception. TraceId: {TraceId}",
            Activity.Current?.Id ?? httpContext.TraceIdentifier);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext 
        { 
            HttpContext = httpContext, 
            ProblemDetails = { Status = 500 } 
        });


       
    }
}

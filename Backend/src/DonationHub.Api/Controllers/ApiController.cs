using DonationHub.Domain.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DonationHub.Api.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected ActionResult Problem(List<Error> errors) 
        {
            if (errors.Count is 0)
            {
                return Problem();
            }

            if (errors.All(error => error.Type == ErrorKind.Validation)) 
            {
                return ValidationProblem(errors);
            }

            return Problem(errors[0]);

        }


        private ObjectResult Problem(Error error) 
        {
            var statusCode = error.Type switch
            {
                ErrorKind.Conflict => StatusCodes.Status409Conflict,
                ErrorKind.Validation => StatusCodes.Status400BadRequest,
                ErrorKind.NotFound => StatusCodes.Status404NotFound,
                ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
                ErrorKind.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
                ErrorKind.PayloadTooLarge => StatusCodes.Status413PayloadTooLarge,         
                ErrorKind.TooManyRequests => StatusCodes.Status429TooManyRequests,         
                _ => StatusCodes.Status500InternalServerError,
            };

            if (statusCode >= 500) 
            {
                HttpContext.RequestServices
                    .GetRequiredService<ILogger<ApiController>>()
                    .LogError("Result failure {Code} :{Description}", error.Code, error.Description);
            }


            var title = statusCode >= 500
                       ? "An unexpected error occurred."
                       : error.Description;

            var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: statusCode, title: title);

            //custom fields":
            problem.Extensions["code"] = error.Code;

            return new ObjectResult(problem) { StatusCode = statusCode };
        }


        private ActionResult ValidationProblem(List<Error> errors) 
        {
            var modelStateDictionary = new ModelStateDictionary();

            errors.ForEach(error => modelStateDictionary.AddModelError(error.Code, error.Description));

            return ValidationProblem(modelStateDictionary);
        }

    }
}

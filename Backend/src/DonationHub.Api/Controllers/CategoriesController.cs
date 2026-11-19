using Asp.Versioning;
using DonationHub.Api.Contracts.Category;
using DonationHub.Application.Features.Categories.Commands.Create;
using DonationHub.Application.Features.Categories.Commands.Delete;
using DonationHub.Application.Features.Categories.Commands.Update;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Application.Features.Categories.Queries.GetAll;
using DonationHub.Application.Features.Categories.Queries.GetById;
using DonationHub.Application.Features.SecurityManager.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    [Route("api/v{version:apiVersion}/categories")]
    [ApiVersion("1.0")]
    [Tags("Categories")]

    public class CategoriesController(ISender sender) : ApiController
    {

        [HttpGet]
        [AllowAnonymous]
        [EndpointSummary("Gets Categories")]
        [EndpointDescription("Returns a list of categories")]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetCategoriesQuery(), cancellationToken);

            return result.IsError
                   ? Problem(result.Errors.ToList())
                   : Ok(result.Value);

        }

        [HttpGet("{id:guid}",Name ="GetCategoryById")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Gets a Category by ID")]
        [EndpointDescription("Returns detailed information about the specified category if found.")]

        public async Task<IActionResult> GetById(
        Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
            return result.IsError ? Problem(result.Errors.ToList()) : Ok(result.Value);
        }




        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [EndpointSummary("Adds a new category to the system.")]
     public async Task<IActionResult> Create(
        CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateCategoryCommand(
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn), cancellationToken);

            return result.IsError
                   ? Problem(result.Errors.ToList())
                   : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }


        [HttpPut("{id:guid}")]
        [Authorize(Roles = RoleNames.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status409Conflict)]
        [EndpointSummary("Updates an existing category.")]
        public async Task<IActionResult> Update(
        Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
        {

            var result = await sender.Send(new UpdateCategoryCommand(
                id,
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn), cancellationToken);


            return result.IsError
                   ? Problem(result.Errors.ToList())
                   : NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = RoleNames.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
            return result.IsError ? Problem(result.Errors.ToList()) : NoContent();
        }
    }
}

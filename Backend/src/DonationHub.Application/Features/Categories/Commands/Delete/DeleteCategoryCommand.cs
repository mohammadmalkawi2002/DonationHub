using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.Delete
{
    public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest<Result>;
   
}

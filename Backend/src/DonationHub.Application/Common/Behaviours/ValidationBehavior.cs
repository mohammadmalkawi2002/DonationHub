using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Common.Results.Abstractions;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Common.Behaviours
{
    public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator=null)
        : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
            where TResponse : IResult
    {

        private readonly IValidator<TRequest>? _validator = validator;


        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            //1 if you do not have any validator ==> stop do not complete
            if (_validator is null)
                return await next(ct);

            //here: means you have validator => encapsulate the req inside a context that fluent validation understand it :

            var context = new ValidationContext<TRequest>(request);


            //2. execute all the validators in parallel and get the result of each validator:

            var validationResult = await _validator.ValidateAsync(context, ct);


            //3. get all the errors from all the validators and convert them to a list of Error (custom):
            var errors = validationResult.Errors
                .Select(error => Error.Validation(
                    code: error.PropertyName,
                    description: error.ErrorMessage))
                .ToList();


            if (errors.Count == 0)
            {
                return await next(ct);
            }

            
            return (dynamic)errors;

        }
    }
}

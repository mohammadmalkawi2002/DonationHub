using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Common.Results.Abstractions;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Common.Behaviours
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
            where TResponse : IResult
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            //1 if you do not have any validator ==> stop do not complete

            if (!_validators.Any())
            {
                return await next(ct);
            }

            //here: means you have validator => encapsulate the req inside a context that fluent validation understand it :

            var context = new ValidationContext<TRequest>(request);


            //2. execute all the validators in parallel and get the result of each validator:
            
            var validationResults = await Task.WhenAll(
            _validators.Select(v =>
                v.ValidateAsync(context, ct)));

            //3. get all the errors from all the validators and convert them to a list of Error (custom):
            var errors = validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error is not null)
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

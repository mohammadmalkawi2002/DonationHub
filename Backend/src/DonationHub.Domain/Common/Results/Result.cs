using DonationHub.Domain.Common.Results.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DonationHub.Domain.Common.Results
{

    public sealed class Result : IResult
    {
        private readonly IReadOnlyList<Error> _errors;

        private Result()
        {
            IsSuccess = true;
            _errors = Array.Empty<Error>();
        }

        private Result(IEnumerable<Error> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);

            var errorList = errors.ToArray();

            if (errorList.Length == 0)
                throw new ArgumentException(
                    "At least one error is required.",
                    nameof(errors));

            _errors = Array.AsReadOnly(errorList);
            IsSuccess = false;
        }

        public bool IsSuccess { get; }

        public bool IsError => !IsSuccess;

        public IReadOnlyList<Error> Errors => _errors;

        public Error TopError =>
            Errors.Count > 0 ? Errors[0] : default;

        /// <summary>
        /// Handles both success and error cases using a functional style.
        /// </summary>
        //public TResult Match<TResult>(
        //    Func<TResult> onSuccess,
        //    Func<IReadOnlyList<Error>, TResult> onError)
        //    => IsSuccess ? onSuccess() : onError(Errors);

        public static Result Success()
            => new();

        public static Result Failure(Error error)
            => new(new[] { error });

        public static Result Failure(IEnumerable<Error> errors)
            => new(errors);

        public static implicit operator Result(List<Error> errors)
            => Failure(errors);
    }

    public sealed class Result<TValue> : IResult<TValue>
        where TValue : notnull
    {
        private readonly TValue? _value = default;
        private readonly IReadOnlyList<Error> _errors = [];

        private Result(TValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            _value = value;
            IsSuccess = true;
        }

        private Result(IEnumerable<Error> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);

            var errorList = errors.ToArray();

            if (errorList.Length == 0)
                throw new ArgumentException(
                    "At least one error is required.",
                    nameof(errors));

            _errors = Array.AsReadOnly(errorList);
            IsSuccess = false;
        }

        public bool IsSuccess { get; }

        public bool IsError => !IsSuccess;

        public IReadOnlyList<Error> Errors => _errors;

        public TValue Value =>
            IsSuccess ? _value! : default!;

        public Error TopError =>
            Errors.Count > 0 ? Errors[0] : default;

        public static Result<TValue> Success(TValue value)
            => new(value);

        public static Result<TValue> Failure(Error error)
            => new(new[] { error });

        public static Result<TValue> Failure(
            IEnumerable<Error> errors)
            => new(errors);

        public TResult Match<TResult>(
           Func<TResult> onSuccess,
           Func<IReadOnlyList<Error>, TResult> onError)
           => IsSuccess ? onSuccess() : onError(Errors);

        public static implicit operator Result<TValue>(
            List<Error> errors)
            => Failure(errors);
    }


   
}

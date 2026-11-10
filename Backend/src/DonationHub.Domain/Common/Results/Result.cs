using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DonationHub.Domain.Common.Results
{
  public sealed class Result 
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
        public Error TopError => Errors.Count > 0 ? Errors[0] : default;

        public TResult Match<TResult>(
               Func<TResult> onSuccess,
               Func<IReadOnlyList<Error>, TResult> onError)
               => IsSuccess ? onSuccess() : onError(Errors);

        public static Result Success()
            => new();

        public static Result Failure(Error error)
            => new(new[] { error });

        public static Result Failure(IEnumerable<Error> errors)
            => new(errors);

        public static Result<TValue> Success<TValue>(TValue value)
            where TValue : notnull
            => Result<TValue>.Success(value);

        public static Result<TValue> Failure<TValue>(Error error)
            where TValue : notnull
            => Result<TValue>.Failure(error);

        public static Result<TValue> Failure<TValue>(IEnumerable<Error> errors)
            where TValue : notnull
            => Result<TValue>.Failure(errors);


    }

    public sealed class Result<TValue>
    where TValue : notnull
    {
        private readonly TValue? _value = default;
        private readonly IReadOnlyList<Error> _errors = [];

        public bool IsSuccess { get; }
        public bool IsError => !IsSuccess;

        public IReadOnlyList<Error> Errors => _errors;

        public TValue Value => IsSuccess ? _value! : default!;

        public Error TopError => Errors.Count > 0 ? Errors[0] : default;

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

        public TResult Match<TResult>(
            Func<TValue, TResult> onValue,
            Func<IReadOnlyList<Error>, TResult> onError)
            => IsSuccess ? onValue(Value) : onError(Errors);

        internal static Result<TValue> Success(TValue value)
            => new(value);

        internal static Result<TValue> Failure(Error error)
            => new(new[] { error });

        internal static Result<TValue> Failure(IEnumerable<Error> errors)
            => new(errors);
    }
}

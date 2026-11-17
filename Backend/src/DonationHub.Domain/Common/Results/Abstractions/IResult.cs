using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common.Results.Abstractions
{
    public interface IResult
    {
        bool IsSuccess { get; }
        IReadOnlyList<Error> Errors { get; }
    }

    public interface IResult<out TValue> : IResult
    {
        TValue Value { get; }
    }
}

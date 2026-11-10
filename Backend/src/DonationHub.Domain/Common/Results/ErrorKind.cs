using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common.Results
{
    public enum ErrorKind
    {
        Failure,
        Unexpected,
        Validation,
        PayloadTooLarge,
        UnprocessableEntity,
        TooManyRequests,
        Conflict,
        NotFound,
        Unauthorized,
        Forbidden,
    }

}

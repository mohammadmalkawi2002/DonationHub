using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common.Interfaces
{
    /// <summary>
    /// Marks an entity as supporting soft deletion and restoration.
    /// Infrastructure can use this contract in the EF Core interceptor.
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeletedAtUtc { get; set; }

    }

}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

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

        public void Delete(DateTimeOffset deletedAtUtc)
        {
            IsDeleted = true;
            DeletedAtUtc = deletedAtUtc;
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAtUtc = null;
        }

    }

}

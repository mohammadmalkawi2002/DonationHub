using DonationHub.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DonationHub.Infrastructure.Persistence.Interceptors
{
    public sealed class SoftDeleteInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }


        private  void ApplySoftDelete(DbContext? context) 
        {
            if (context is null)
                return;

            // Get all entities that implement ISoftDeletable and are marked for deletion
            var entries =context.ChangeTracker
                        .Entries<ISoftDeletable>()
                        .Where(entry=>entry.State == EntityState.Deleted);

            var nowUtc = timeProvider.GetUtcNow(); // Get the current UTC time


            foreach (var entry in entries) 
            { 
                entry.Entity.Delete(nowUtc); // Mark the entity as soft-deleted

                entry.State = EntityState.Modified; // Change the state to Modified so that EF Core updates the entity instead of deleting it
            }
        }
    }
}

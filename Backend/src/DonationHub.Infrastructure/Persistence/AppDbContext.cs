
using DonationHub.Domain.Entities;
using DonationHub.Domain.Entities.Campaigns;
using DonationHub.Domain.Entities.Categories;
using DonationHub.Domain.Entities.Donations;
using DonationHub.Domain.Entities.Identity;
using DonationHub.Domain.Entities.PaymentAttempts;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DonationHub.Infrastructure.Persistence
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
        :  IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Campaign> Campaigns => Set<Campaign>();

        public DbSet<Donation> Donations => Set<Donation>();

        public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();

        public DbSet<ProcessedStripeEvent> ProcessedStripeEvents => Set<ProcessedStripeEvent>();

        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

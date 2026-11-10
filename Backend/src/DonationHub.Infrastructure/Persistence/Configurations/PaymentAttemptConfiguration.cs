using DonationHub.Domain.Entities.Donations;
using DonationHub.Domain.Entities.PaymentAttempts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public sealed class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
    {
        public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
        {
            builder.ToTable("PaymentAttempts");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.DonationId).IsRequired();

            builder.Property(x => x.StripeCheckoutSessionId)
                .IsRequired();

            builder.Property(x => x.StripePaymentIntentId);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.FailureCode);

            builder.OwnsOne(x => x.Amount, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.HasOne<Donation>()
         .WithMany()
         .HasForeignKey(x => x.DonationId)
         .OnDelete(DeleteBehavior.Restrict);


            //later we can add indexes to improve query performance, but for now, we will leave them commented out
            //builder.HasIndex(x => x.DonationId);
            //builder.HasIndex(x => x.StripeCheckoutSessionId).IsUnique();
            //builder.HasIndex(attempt => attempt.StripePaymentIntentId);


        }
    }
}

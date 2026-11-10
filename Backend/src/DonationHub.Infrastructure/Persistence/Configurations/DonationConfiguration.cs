using DonationHub.Domain.Entities.Campaigns;
using DonationHub.Domain.Entities.Donations;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public sealed class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("Donations");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.CampaignId).IsRequired();
            builder.Property(x => x.DonorUserId).IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.CompletedAt);
            builder.Property(x => x.FailureReason);
            builder.Property(x => x.ReceiptNumber);
            builder.Property(x => x.ReceiptFileName);

            builder.OwnsOne(x => x.Amount, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("DonationAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("DonationCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });


            builder.HasOne<Campaign>()
           .WithMany()
           .HasForeignKey(x => x.CampaignId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
           .WithMany()
           .HasForeignKey(x => x.DonorUserId)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

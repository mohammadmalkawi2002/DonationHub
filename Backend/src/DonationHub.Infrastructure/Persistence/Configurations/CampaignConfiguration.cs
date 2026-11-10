using DonationHub.Domain.Entities.Campaigns;
using DonationHub.Domain.Entities.Categories;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.TitleAr).IsRequired();
            builder.Property(x => x.TitleEn).IsRequired();
            builder.Property(x => x.DescriptionAr);
            builder.Property(x => x.DescriptionEn);

            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.CategoryId).IsRequired();
            builder.Property(x => x.BannerFileName);

            builder.Property(x=>x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.CreatedByUserId).IsRequired();

            builder.Property(x => x.RowVersion)
           .IsRowVersion();

            builder.OwnsOne(x=>x.GoalAmount, money =>
            {
                money.Property(x => x.Amount)
                .HasColumnName("GoalAmount")
                .HasPrecision(18, 2)
                .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("GoalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });


            builder.OwnsOne(x => x.RaisedAmount, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("RaisedAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("RaisedCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Configure the relationship between Campaign and Category
            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x=>x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configure the relationship between Campaign and ApplicationUser (CreatedByUserId): one Campaign is created by one ApplicationUser, and one ApplicationUser can create many Campaigns.
            builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);



            // Configure indexes for frequently queried properties later for performance optimization:
            //builder.HasIndex(campaign => campaign.CategoryId);
            //builder.HasIndex(campaign => campaign.CreatedByUserId);
            //builder.HasIndex(campaign => new { campaign.Status, campaign.EndDate });






        }
    }
}

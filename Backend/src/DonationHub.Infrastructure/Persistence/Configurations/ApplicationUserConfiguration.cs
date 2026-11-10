using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(user => user.FullName)
           .IsRequired();

            builder.Property(user => user.PreferredLanguage)
                .IsRequired()
                .HasMaxLength(2);

            builder.HasIndex(user => user.NormalizedEmail)
    .IsUnique()
    .HasFilter("[NormalizedEmail] IS NOT NULL");
        }
    }
}

using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public sealed class ProcessedStripeEventConfiguration : IEntityTypeConfiguration<ProcessedStripeEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedStripeEvent> builder)
        {
            builder.ToTable("ProcessedStripeEvents");

            builder.HasKey(processedEvent => processedEvent.EventId);
            builder.Property(processedEvent => processedEvent.EventId).HasMaxLength(256).IsRequired();
            builder.Property(processedEvent => processedEvent.Type).HasMaxLength(256).IsRequired();
            builder.Property(processedEvent => processedEvent.ProcessedAt).IsRequired();
        }
    }
}

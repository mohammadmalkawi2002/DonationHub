using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyRecords");


            //every record is uniquely identified by the combination of UserId and Key
            //هذا يضمن أن نفس المستخدم ما يقدر يسجل نفس المفتاح مرتين، بينما مستخدم آخر ممكن يستخدم نفس المفتاح.
            builder.HasKey(record => new { record.UserId, record.Key });

            builder.Property(record => record.Key)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(record => record.UserId).IsRequired();

            builder.Property(record => record.RequestHash)
                .IsRequired();

            builder.Property(record => record.ResponseStatusCode)
                .IsRequired();

            builder.Property(record => record.ResponseBody)
                .IsRequired();

            builder.Property(record => record.CreatedAtUtc)
                .IsRequired();

            builder.Property(record => record.ExpiresAt)
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(record => record.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

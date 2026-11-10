using DonationHub.Domain.Entities.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(category => category.Id);
            builder.Property(category => category.Id).ValueGeneratedNever();

            builder.Property(category => category.NameAr).IsRequired().HasMaxLength(100);

            builder.Property(category => category.NameEn).IsRequired().HasMaxLength(100);

            //to ensure that the category names are unique, 
            builder.HasIndex(category => category.NameAr).IsUnique();
            builder.HasIndex(category => category.NameEn).IsUnique();

            builder.Property(category => category.DescriptionAr);
            builder.Property(category => category.DescriptionEn);
        }
    }
}

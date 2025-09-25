using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealWorldUnitTest.Web.Models;

namespace RealWorldUnitTest.Web.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Stock).IsRequired();
        builder.Property(x=>x.Color).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.ToTable("Products");
    }
}

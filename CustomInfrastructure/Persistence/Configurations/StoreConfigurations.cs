using CustomDomain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInfrastructure.Persistence.Configurations
{
    public class StoreProductConfiguration : IEntityTypeConfiguration<StoreProduct>
    {
        public void Configure(EntityTypeBuilder<StoreProduct> b)
        {
            b.Property(x => x.StoreName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.PriceUsd).HasPrecision(18, 2);
            b.Property(x => x.WeightKg).HasPrecision(18, 3);
            b.HasOne(x => x.HsCode).WithMany().HasForeignKey(x => x.HsCodeId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class StoreOrderConfiguration : IEntityTypeConfiguration<StoreOrder>
    {
        public void Configure(EntityTypeBuilder<StoreOrder> b)
        {
            b.Property(x => x.OrderNumber).HasMaxLength(30).IsRequired();
            b.HasIndex(x => x.OrderNumber).IsUnique();
            b.Property(x => x.TotalAmount).HasPrecision(18, 2);
            b.HasIndex(x => x.UserId);
            b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Items).WithOne(i => i.Order)
                .HasForeignKey(i => i.StoreOrderId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class StoreOrderItemConfiguration : IEntityTypeConfiguration<StoreOrderItem>
    {
        public void Configure(EntityTypeBuilder<StoreOrderItem> b)
        {
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.HasOne(x => x.Product).WithMany()
                .HasForeignKey(x => x.StoreProductId).OnDelete(DeleteBehavior.Restrict);
        }
    }

}

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
    public class CargoParcelConfiguration : IEntityTypeConfiguration<CargoParcel>
    {
        public void Configure(EntityTypeBuilder<CargoParcel> b)
        {
            b.Property(x => x.TrackingNumber).HasMaxLength(30).IsRequired();
            b.HasIndex(x => x.TrackingNumber).IsUnique();
            b.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            b.HasIndex(x => new { x.OwnerUserId, x.Status });

            // Bir sifariş = bir bağlama
            b.HasOne(x => x.Order).WithOne(o => o.Parcel)
                .HasForeignKey<CargoParcel>(x => x.StoreOrderId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Owner).WithMany()
                .HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Warehouse).WithMany()
                .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.CargoCompany).WithMany()
                .HasForeignKey(x => x.CargoCompanyId).OnDelete(DeleteBehavior.Restrict);

            // Bir bəyannamə yalnız bir bağlamaya bağlana bilər (təkrar bəyanın DB səviyyəsində qarşısı)
            b.HasOne(x => x.Declaration).WithMany()
                .HasForeignKey(x => x.DeclarationId).OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => x.DeclarationId).IsUnique().HasFilter("[DeclarationId] IS NOT NULL");
        }
    }

    public class CargoCompanyConfiguration : IEntityTypeConfiguration<CargoCompany>
    {
        public void Configure(EntityTypeBuilder<CargoCompany> b)
        {
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
            b.HasOne(x => x.Warehouse).WithMany()
                .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

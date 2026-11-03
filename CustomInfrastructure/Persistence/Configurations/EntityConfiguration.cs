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

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.Property(x => x.Email).HasMaxLength(200).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            b.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            b.Property(x => x.TaxId).HasMaxLength(20);
            b.Property(x => x.Role).HasConversion<string>().HasMaxLength(30);
            b.Property(x => x.DeclarantType).HasConversion<string>().HasMaxLength(30);
        }
    }

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> b)
        {
            b.Property(x => x.TokenHash).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.TokenHash).IsUnique();
            b.HasOne(x => x.User).WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> b)
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Address).HasMaxLength(500).IsRequired();
        }
    }

    public class HsCodeConfiguration : IEntityTypeConfiguration<HsCode>
    {
        public void Configure(EntityTypeBuilder<HsCode> b)
        {
            b.Property(x => x.Code).HasMaxLength(10).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.Description).HasMaxLength(500).IsRequired();
            b.Property(x => x.DutyRate).HasPrecision(5, 4);
            b.Property(x => x.VatRate).HasPrecision(5, 4);
        }
    }

    public class DeclarationConfiguration : IEntityTypeConfiguration<Declaration>
    {
        public void Configure(EntityTypeBuilder<Declaration> b)
        {
            b.Property(x => x.Number).HasMaxLength(30).IsRequired();
            b.HasIndex(x => x.Number).IsUnique();
            b.HasIndex(x => x.Status);
            b.HasIndex(x => new { x.DeclarantUserId, x.Status });   

            b.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            b.Property(x => x.DeclarantType).HasConversion<string>().HasMaxLength(30);

            b.Property(x => x.TotalCustomsValue).HasPrecision(18, 2);
            b.Property(x => x.ExemptAmount).HasPrecision(18, 2);
            b.Property(x => x.TotalDuty).HasPrecision(18, 2);
            b.Property(x => x.TotalVat).HasPrecision(18, 2);

            b.HasOne(x => x.Declarant).WithMany()
                .HasForeignKey(x => x.DeclarantUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Warehouse).WithMany()
                .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Items).WithOne(i => i.Declaration)
                .HasForeignKey(i => i.DeclarationId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.History).WithOne(h => h.Declaration)
                .HasForeignKey(h => h.DeclarationId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class DeclarationItemConfiguration : IEntityTypeConfiguration<DeclarationItem>
    {
        public void Configure(EntityTypeBuilder<DeclarationItem> b)
        {
            b.Property(x => x.Description).HasMaxLength(500).IsRequired();
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.Property(x => x.WeightKg).HasPrecision(18, 3);
            b.Property(x => x.DutyAmount).HasPrecision(18, 2);
            b.Property(x => x.VatAmount).HasPrecision(18, 2);
            b.HasOne(x => x.HsCode).WithMany()
                .HasForeignKey(x => x.HsCodeId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class StatusHistoryConfiguration : IEntityTypeConfiguration<DeclarationStatusHistory>
    {
        public void Configure(EntityTypeBuilder<DeclarationStatusHistory> b)
        {
            b.Property(x => x.FromStatus).HasConversion<string>().HasMaxLength(30);
            b.Property(x => x.ToStatus).HasConversion<string>().HasMaxLength(30);
            b.Property(x => x.Reason).HasMaxLength(500);
            b.HasOne(x => x.ChangedBy).WithMany()
                .HasForeignKey(x => x.ChangedByUserId).OnDelete(DeleteBehavior.Restrict);
        }
    }

}

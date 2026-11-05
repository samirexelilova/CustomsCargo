using CustomDomain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInfrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Declaration> Declarations => Set<Declaration>();
        public DbSet<DeclarationItem> DeclarationItems => Set<DeclarationItem>();
        public DbSet<DeclarationStatusHistory> StatusHistory => Set<DeclarationStatusHistory>();
        public DbSet<HsCode> HsCodes => Set<HsCode>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();
        public DbSet<StoreOrder> StoreOrders => Set<StoreOrder>();
        public DbSet<StoreOrderItem> StoreOrderItems => Set<StoreOrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.HasSequence<int>("DeclarationNumbers").StartsAt(1).IncrementsBy(1);

            foreach (var entity in modelBuilder.Model.GetEntityTypes()
                         .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
                modelBuilder.Entity(entity.ClrType).Property(nameof(BaseEntity.Id)).ValueGeneratedNever();
        }
    }
}

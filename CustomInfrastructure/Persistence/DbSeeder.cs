using CustomApplication.Interfaces;
using CustomDomain.Entities;
using CustomDomain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInfrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            var db = sp.GetRequiredService<AppDbContext>();
            var hasher = sp.GetRequiredService<IPasswordHasher>();
            var config = sp.GetRequiredService<IConfiguration>();
            var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

            if (!await db.HsCodes.AnyAsync())
            {
                db.HsCodes.AddRange(
                    new HsCode { Code = "8517130000", Description = "Smartfonlar", DutyRate = 0.05m, VatRate = 0.18m },
                    new HsCode { Code = "8471300000", Description = "Noutbuklar", DutyRate = 0.00m, VatRate = 0.18m },
                    new HsCode { Code = "6109100000", Description = "Pambıq köynəklər", DutyRate = 0.15m, VatRate = 0.18m },
                    new HsCode { Code = "6403990000", Description = "Dəri ayaqqabılar", DutyRate = 0.15m, VatRate = 0.18m },
                    new HsCode { Code = "8528720000", Description = "Televizorlar", DutyRate = 0.10m, VatRate = 0.18m });
            }

            if (!await db.Warehouses.AnyAsync())
            {
                db.Warehouses.AddRange(
                    new Warehouse { Name = "Bakı Gömrük Anbarı", Address = "Bakı, Binəqədi" },
                    new Warehouse { Name = "Sumqayıt Gömrük Anbarı", Address = "Sumqayıt" });
            }

            var password = config["Seed:Password"];
            if (string.IsNullOrWhiteSpace(password))
            {
                log.LogWarning("Seed:Password təyin olunmayıb, test istifadəçiləri yaradılmadı");
            }
            else if (!await db.Users.AnyAsync())
            {
                var hash = hasher.Hash(password);   
                db.Users.AddRange(
                    new User { Email = "admin@customs.local", FullName = "Sistem Admin", Role = UserRole.Admin, PasswordHash = hash },
                    new User { Email = "officer@customs.local", FullName = "Gömrük Əməkdaşı", Role = UserRole.CustomsOfficer, PasswordHash = hash },
                    new User { Email = "warehouse@customs.local", FullName = "Anbar İşçisi", Role = UserRole.WarehouseOperator, PasswordHash = hash },
                    new User
                    {
                        Email = "individual@customs.local",
                        FullName = "Fiziki Şəxs Deklarant",
                        Role = UserRole.Declarant,
                        DeclarantType = DeclarantType.Individual,
                        PasswordHash = hash
                    },
                    new User
                    {
                        Email = "company@customs.local",
                        FullName = "Baku Trade LLC",
                        Role = UserRole.Declarant,
                        DeclarantType = DeclarantType.LegalEntity,
                        TaxId = "1234567890",
                        PasswordHash = hash
                    });
            }

            await db.SaveChangesAsync();
        }
    }

}

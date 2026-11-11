using CustomApplication.Auth;
using CustomApplication.Interfaces;
using CustomInfrastructure.Auth;
using CustomInfrastructure.Persistence;
using CustomInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInfrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection tapılmadı. User Secrets ilə təyin edin (README).");

            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));
            services.Configure<JwtOptions>(config.GetSection("Jwt"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDeclarationRepository, DeclarationRepository>();
            services.AddScoped<IHsCodeRepository, HsCodeRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<ICargoParcelRepository, CargoParcelRepository>();
            services.AddScoped<ICargoCompanyRepository, CargoCompanyRepository>();

            services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            return services;
        }
    }

}

using CustomApplication.Interfaces;
using CustomApplication.Services;
using CustomDomain.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<CustomsDutyCalculator>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDeclarationService, DeclarationService>();
            services.AddScoped<IDutyService, DutyService>();
            services.AddScoped<IReferenceDataService, ReferenceDataService>();

            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<ICargoService, CargoService>();
            return services;
        }
    }

}

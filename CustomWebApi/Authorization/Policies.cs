using CustomDomain.Enums;

namespace CustomWebApi.Authorization
{
    public static class Policies
    {
        public const string DeclarantOnly = nameof(DeclarantOnly);
        public const string StaffOnly = nameof(StaffOnly);
        public const string AdminOnly = nameof(AdminOnly);

        public static void Register(Microsoft.AspNetCore.Authorization.AuthorizationOptions o)
        {
            o.AddPolicy(DeclarantOnly, p => p.RequireRole(nameof(UserRole.Declarant)));
            o.AddPolicy(StaffOnly, p => p.RequireRole(
                nameof(UserRole.CustomsOfficer), nameof(UserRole.WarehouseOperator), nameof(UserRole.Admin)));
            o.AddPolicy(AdminOnly, p => p.RequireRole(nameof(UserRole.Admin)));
        }
    }

}

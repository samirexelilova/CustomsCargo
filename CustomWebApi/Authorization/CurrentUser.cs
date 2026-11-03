using CustomApplication.Interfaces;
using CustomDomain.Enums;
using System.Security.Claims;

namespace CustomWebApi.Authorization
{
    public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
    {
        private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

        public Guid? UserId =>
            Guid.TryParse(Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

        public UserRole? Role =>
            Enum.TryParse<UserRole>(Principal?.FindFirst(ClaimTypes.Role)?.Value, out var role) ? role : null;
    }
}

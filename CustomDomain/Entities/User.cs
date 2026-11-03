using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class User : BaseEntity
    {
        public const int MaxFailedAttempts = 5;
        public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DeclarantType? DeclarantType { get; set; }   // yalnız Deklarant üçün
        public string? TaxId { get; set; }                  // VÖEN / FİN
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public int FailedLoginCount { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public bool IsLockedOut(DateTimeOffset now) => LockoutEnd is { } end && end > now;

        public void RegisterFailedLogin(DateTimeOffset now)
        {
            FailedLoginCount++;
            if (FailedLoginCount >= MaxFailedAttempts)
            {
                LockoutEnd = now.Add(LockoutDuration);
                FailedLoginCount = 0;
            }
        }

        public void ResetLoginFailures()
        {
            FailedLoginCount = 0;
            LockoutEnd = null;
        }
    }
}

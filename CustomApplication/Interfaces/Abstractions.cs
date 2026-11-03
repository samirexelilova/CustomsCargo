using CustomDomain.Entities;
using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct);
    }

    public interface ICurrentUser
    {
        Guid? UserId { get; }
        UserRole? Role { get; }
    }

    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string storedHash);
    }

    public record AccessToken(string Token, DateTimeOffset ExpiresAt);

    public interface IJwtTokenService
    {
        AccessToken CreateAccessToken(User user);
        (string Raw, string Hash) CreateRefreshToken();
        string HashToken(string rawToken);
    }

}

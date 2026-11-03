using CustomApplication.Interfaces;
using CustomDomain.Entities;
using CustomInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInfrastructure.Repositories
{
    public class UserRepository(AppDbContext db) : IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
            => db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
            => db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct)
            => db.Users.AnyAsync(u => u.Email == email, ct);

        public void Add(User user) => db.Users.Add(user);

        public void AddRefreshToken(RefreshToken token) => db.RefreshTokens.Add(token);

        public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct)
            => db.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

        public Task RevokeAllRefreshTokensAsync(Guid userId, DateTimeOffset now, CancellationToken ct)
            => db.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, (DateTimeOffset?)now), ct);
    }

}

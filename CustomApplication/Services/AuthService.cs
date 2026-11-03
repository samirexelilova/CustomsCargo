using CustomApplication.Auth;
using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomDomain.Entities;
using CustomDomain.Enums;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomApplication.Services
{
    public class AuthService(
     IUserRepository users,
     IPasswordHasher hasher,
     IJwtTokenService jwt,
     IUnitOfWork uow,
     IOptions<JwtOptions> jwtOptions,
     TimeProvider clock,
     ICurrentUser currentUser) : IAuthService
    {
        private const string InvalidCredentials = "E-poçt və ya parol səhvdir.";

        public async Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct)
        {
            var email = NormalizeEmail(req.Email);
            var errors = new ErrorCollector();
            ValidateCommon(errors, email, req.Password, req.FullName);
            if (!Enum.IsDefined(req.DeclarantType))
                errors.Add("declarantType", "Deklarant növü yanlışdır.");
            if (req.DeclarantType == DeclarantType.LegalEntity && !Regex.IsMatch(req.TaxId ?? "", @"^\d{10}$"))
                errors.Add("taxId", "Hüquqi şəxs üçün 10 rəqəmli VÖEN tələb olunur.");
            errors.ThrowIfAny();

            if (await users.EmailExistsAsync(email, ct))
                throw new ConflictException("Bu e-poçt artıq qeydiyyatdan keçib.");

            // Açıq qeydiyyat yalniz Deklarant yaradır Əməkdaşları yalnız Admin yarada bilər
            var user = new User
            {
                Email = email,
                PasswordHash = hasher.Hash(req.Password),
                FullName = req.FullName.Trim(),
                Role = UserRole.Declarant,
                DeclarantType = req.DeclarantType,
                TaxId = req.TaxId?.Trim()
            };
            users.Add(user);

            var response = IssueTokens(user);
            await uow.SaveChangesAsync(ct);
            return response;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct)
        {
            var now = clock.GetUtcNow();
            var user = await users.GetByEmailAsync(NormalizeEmail(req.Email), ct);

            if (user is null || !user.IsActive)
                throw new UnauthorizedAppException(InvalidCredentials);

            if (user.IsLockedOut(now))
                throw new LockedOutException("Çox sayda uğursuz cəhd. Bir az sonra yenidən yoxlayın.");

            if (!hasher.Verify(req.Password ?? string.Empty, user.PasswordHash))
            {
                user.RegisterFailedLogin(now);
                await uow.SaveChangesAsync(ct);
                throw new UnauthorizedAppException(InvalidCredentials);
            }

            user.ResetLoginFailures();
            var response = IssueTokens(user);
            await uow.SaveChangesAsync(ct);
            return response;
        }

        public async Task<AuthResponse> RefreshAsync(RefreshRequest req, CancellationToken ct)
        {
            var now = clock.GetUtcNow();
            var stored = await users.GetRefreshTokenAsync(jwt.HashToken(req.RefreshToken ?? string.Empty), ct)
                ?? throw new UnauthorizedAppException("Token yanlışdır.");

            if (stored.RevokedAt is not null)
            {
                await users.RevokeAllRefreshTokensAsync(stored.UserId, now, ct);
                throw new UnauthorizedAppException("Token yanlışdır.");
            }

            if (!stored.IsActive(now) || !stored.User.IsActive || stored.User.IsLockedOut(now))
                throw new UnauthorizedAppException("Token müddəti bitib.");

            stored.RevokedAt = now;
            var response = IssueTokens(stored.User);
            await uow.SaveChangesAsync(ct);
            return response;
        }

        public async Task LogoutAsync(RefreshRequest req, CancellationToken ct)
        {
            var stored = await users.GetRefreshTokenAsync(jwt.HashToken(req.RefreshToken ?? string.Empty), ct);
            if (stored is { RevokedAt: null })
            {
                stored.RevokedAt = clock.GetUtcNow();
                await uow.SaveChangesAsync(ct);
            }
        }

        public async Task<UserDto> CreateStaffAsync(CreateStaffRequest req, CancellationToken ct)
        {
            var email = NormalizeEmail(req.Email);
            var errors = new ErrorCollector();
            ValidateCommon(errors, email, req.Password, req.FullName);
            if (!Enum.IsDefined(req.Role) || req.Role == UserRole.Declarant)
                errors.Add("role", "Əməkdaş rolu seçilməlidir (CustomsOfficer, WarehouseOperator, Admin).");
            errors.ThrowIfAny();

            if (await users.EmailExistsAsync(email, ct))
                throw new ConflictException("Bu e-poçt artıq mövcuddur.");

            var user = new User
            {
                Email = email,
                PasswordHash = hasher.Hash(req.Password),
                FullName = req.FullName.Trim(),
                Role = req.Role
            };
            users.Add(user);
            await uow.SaveChangesAsync(ct);
            return ToDto(user);
        }

        public async Task<UserDto> GetCurrentAsync(CancellationToken ct)
        {
            var id = currentUser.UserId ?? throw new UnauthorizedAppException("Giriş tələb olunur.");
            var user = await users.GetByIdAsync(id, ct) ?? throw new UnauthorizedAppException("İstifadəçi tapılmadı.");
            return ToDto(user);
        }

        private AuthResponse IssueTokens(User user)
        {
            var now = clock.GetUtcNow();
            var access = jwt.CreateAccessToken(user);
            var (raw, hash) = jwt.CreateRefreshToken();

            users.AddRefreshToken(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = hash,
                CreatedAt = now,
                ExpiresAt = now.AddDays(jwtOptions.Value.RefreshTokenDays)
            });

            return new AuthResponse(access.Token, access.ExpiresAt, raw, ToDto(user));
        }

        private static UserDto ToDto(User u) => new(u.Id, u.Email, u.FullName, u.Role, u.DeclarantType);

        private static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

        private static void ValidateCommon(ErrorCollector e, string email, string? password, string? fullName)
        {
            if (!MailAddress.TryCreate(email, out _) || email.Length > 200)
                e.Add("email", "E-poçt formatı yanlışdır.");

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length is < 2 or > 150)
                e.Add("fullName", "Ad Soyad 2-150 simvol olmalıdır.");

            if (string.IsNullOrEmpty(password) || password.Length is < 8 or > 100
                || !password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
                e.Add("password", "Parol 8-100 simvol olmalı, böyük hərf, kiçik hərf və rəqəm daxil etməlidir.");
        }
    }

}

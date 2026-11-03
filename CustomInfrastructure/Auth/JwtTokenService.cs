using CustomApplication.Auth;
using CustomApplication.Interfaces;
using CustomDomain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CustomInfrastructure.Auth
{
    public class JwtTokenService(IOptions<JwtOptions> options, TimeProvider clock) : IJwtTokenService
    {
        private readonly JwtOptions _o = options.Value;

        public AccessToken CreateAccessToken(User user)
        {
            var now = clock.GetUtcNow();
            var expires = now.AddMinutes(_o.AccessTokenMinutes);

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_o.Key));
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _o.Issuer,
                Audience = _o.Audience,
                NotBefore = now.UtcDateTime,
                Expires = expires.UtcDateTime,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            return new AccessToken(handler.WriteToken(handler.CreateToken(descriptor)), expires);
        }

        public (string Raw, string Hash) CreateRefreshToken()
        {
            var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return (raw, HashToken(raw));
        }

        public string HashToken(string rawToken)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
    }

}

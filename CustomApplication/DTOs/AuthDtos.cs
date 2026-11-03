using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.DTOs
{
    public record RegisterRequest(string Email, string Password, string FullName, DeclarantType DeclarantType, string? TaxId);
    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string RefreshToken);
    public record CreateStaffRequest(string Email, string Password, string FullName, UserRole Role);

    public record UserDto(Guid Id, string Email, string FullName, UserRole Role, DeclarantType? DeclarantType);

    public record AuthResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAt, string RefreshToken, UserDto User);

}

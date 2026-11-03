using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomWebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomWebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService auth) : ControllerBase
    {
        //Deklarant qeydiyyatı 
        [HttpPost("register"), AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
            => Ok(await auth.RegisterAsync(request, ct));

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
            => Ok(await auth.LoginAsync(request, ct));

        [HttpPost("refresh"), AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken ct)
            => Ok(await auth.RefreshAsync(request, ct));

        [HttpPost("logout"), AllowAnonymous]
        public async Task<IActionResult> Logout(RefreshRequest request, CancellationToken ct)
        {
            await auth.LogoutAsync(request, ct);
            return NoContent();
        }

        [HttpGet("me"), Authorize]
        public async Task<ActionResult<UserDto>> Me(CancellationToken ct) => Ok(await auth.GetCurrentAsync(ct));

        //Gömrük əməkdaşı / Anbar işçisi / Admin yaratmaq
        [HttpPost("staff"), Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<UserDto>> CreateStaff(CreateStaffRequest request, CancellationToken ct)
            => StatusCode(StatusCodes.Status201Created, await auth.CreateStaffAsync(request, ct));
    }

}

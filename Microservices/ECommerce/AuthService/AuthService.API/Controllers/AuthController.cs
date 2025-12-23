using AuthService.Application.Services.RefreshTokenService;
using AuthService.Application.Services.UserService;
using AuthService.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthController(
            IUserService userService,
            IRefreshTokenService refreshTokenService)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegistration dto)
        {
            var result = await _userService.RegisterUserAsync(dto);

            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginCredentials credentials)
        {
            var result = await _userService.ValidateUserAsync(credentials);

            if (!result.Success)
                return Unauthorized(result.Error);

            return Ok(new
            {
                accessToken = result.Value
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request)
        {
            var principal = HttpContext.User;

            if (principal?.Identity?.IsAuthenticated != true)
                return Unauthorized();

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Unauthorized();

            var user = await _userService.GetByIdAsync(Guid.Parse(userId));

            //if (user is null || !user.IsActive)
            //    return Unauthorized();

            var tokenResponse = await _refreshTokenService
                .RefreshAsync(request.RefreshToken, null);

            return Ok(tokenResponse);
        }

    }


}

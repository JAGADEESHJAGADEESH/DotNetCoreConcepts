using AuthService.Application.Services.UserService;
using AuthService.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
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

    }


}

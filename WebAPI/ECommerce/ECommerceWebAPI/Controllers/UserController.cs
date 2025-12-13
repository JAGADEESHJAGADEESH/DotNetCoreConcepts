using Ecommerce.Application.Services.PasswordService;
using Ecommerce.Application.Services.TokenService;
using Ecommerce.Application.Services.UserService;
using Ecommerce.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UserController> _logger;

        public UserController(ITokenService tokenService, IUserService userService, IPasswordService passwordService, ILogger<UserController> logger)
        {
            _tokenService = tokenService;
            _userService = userService;
            _passwordService = passwordService;
            _logger = logger;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveUser([FromBody] Users user)
        {
            if (user is null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            try
            {
                var email = user.Email.Trim();
                // Hash password using injected password service and clear plaintext password before saving
                user.PasswordSalt = _passwordService.HashPassword(user.Password.Trim());
                user.Password = null!;
                // Check existing user (repository implementation decides matching logic)
                var existingUser = await _userService.GetUserByUserCredentialsAsync(email);
                if (existingUser is not null)
                {
                    return Conflict(new { message = $"UserName: {existingUser.Email} already exists." });
                }

                var userId = await _userService.SaveUserAsync(user);
                if (userId is null || userId <= 0)
                {
                    return StatusCode(500, new { message = "Failed to register user." });
                }
                var token = await _tokenService.GenerateTokenAsync(user);
                var response = new RegisterResponse
                {
                    Success = true,
                    User = user,
                    Token = token,
                    Message = "Registration successful"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] PublicUser credentials)
        {
            if (credentials is null || string.IsNullOrWhiteSpace(credentials.UserName) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }
            var userName = credentials.UserName.Trim();
            var passwordSalt = _passwordService.HashPassword(credentials.Password.Trim());

            var user = await _userService.GetUserByUserCredentialsAsync(userName);
            if (user is null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Verify hashed password stored on the user record
            try
            {
                if (string.IsNullOrEmpty(user.PasswordSalt) || !_passwordService.VerifyPassword(user.PasswordSalt, credentials.Password))
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                var token = await _tokenService.GenerateTokenAsync(user);
                var response = new RegisterResponse
                {
                    Success = true,
                    User = user,
                    Token = token,
                    Message = "Login successful"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token generation or password verification.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUserByUserCredential(string email)
        {
            try
            {
                var existingUser = await _userService.GetUserByUserCredentialsAsync(email);
                if (existingUser is null)
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }
                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user.");
                return StatusCode(500, new { message = "Internal server error." });
            }

        }
    }
}

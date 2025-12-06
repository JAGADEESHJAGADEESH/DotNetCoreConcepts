using Ecommerce.Application.Services.PasswordService;
using Ecommerce.Application.Services.TokenService;
using Ecommerce.Application.Services.UserService;
using Ecommerce.Core.Models;
using ECommerce.Infrastructure.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IConfiguration configuration,
            ITokenService tokenService,
            IUserService userService,
            IPasswordService passwordService,
            ILogger<UserController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveUser([FromBody] Users user)
        {
            if (user is null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var userName = user.UserName.Trim().ToLowerInvariant();

            try
            {
                // Check existing user (repository implementation decides matching logic)
                var existingUser = await _userService.GetUserByUserCredentialsAsync(userName, user.Password);
                if (existingUser is not null)
                {
                    return Conflict(new { message = "Username already exists." });
                }

                // Hash password using injected password service and clear plaintext password before saving
                user.PasswordSalt = _passwordService.HashPassword(user.Password);
                user.Password = null!;

                await _userService.SaveUserAsync(user);

                // Return CreatedAtAction pointing to the real GET action and supply route values
                return CreatedAtAction(nameof(SaveUser), new { id = user.UserId }, user);
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

            var user = await GetUserByUserCredential(userName, passwordSalt);
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

                var tokenDetails = await _tokenService.GenerateTokenAsync(user);
                if (tokenDetails is not null)
                {
                    var accessToken = tokenDetails.AccessToken;
                    var refreshToken = tokenDetails.RefreshToken;
                    return Ok(new { accessToken, refreshToken });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token generation or password verification.");
                return StatusCode(500, new { message = "Internal server error." });
            }

            return BadRequest();
        }

        [HttpGet("GetUser")]
        public async Task<Users?> GetUserByUserCredential(string userName, string passwordSalt)
        {
            try
            {
                return await _userService.GetUserByUserCredentialsAsync(userName, passwordSalt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user.");
                return null;
            }
        }
    }
}

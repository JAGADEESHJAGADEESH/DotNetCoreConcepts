using Ecommerce.Application.Services.TokenService;
using Ecommerce.Core.Models;
using ECommerce.Infrastructure.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration configuration, ITokenService tokenService, IUserRepository userRepository, ILogger<UserController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public record PublicUser(int Id, string Username);

        // GET api/user/{id}
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user is null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var result = new PublicUser(user.UserId, user.UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Users credentials)
        {
            if (credentials is null || string.IsNullOrWhiteSpace(credentials.UserName) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var userName = credentials.UserName.Trim().ToLowerInvariant();

            try
            {
                // Check existing user (repository implementation decides matching logic)
                var existingUser = await _userRepository.GetUserByUserCredentialsAsync(credentials.UserName, credentials.Password );
                if (existingUser is not null)
                {
                    return Conflict(new { message = "Username already exists." });
                }

                credentials.PasswordSalt = ComputeSha256Hash(credentials.Password);

                await _userRepository.AddUserAsync(credentials);


                // Return CreatedAtAction pointing to the real GET action and supply route values
                return CreatedAtAction(nameof(GetById), new { id = credentials.UserId }, credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Users credentials)
        {
            if (credentials is null || string.IsNullOrWhiteSpace(credentials.UserName) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var user = await GetUser(credentials);
            if (user is null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            try
            {
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
                _logger.LogError(ex, "Error during token generation.");
                return StatusCode(500, new { message = "Internal server error." });
            }

            return BadRequest();
        }

        // Helper - not an API action
        [NonAction]
        public async Task<Users?> GetUser(Users credentials)
        {
            try
            {
                return await _userRepository.GetUserByUserCredentialsAsync(credentials.UserName, credentials.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user.");
                return null;
            }
        }

        private static string ComputeSha256Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

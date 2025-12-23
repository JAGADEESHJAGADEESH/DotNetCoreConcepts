using AuthService.Application.Services.PasswordService;
using AuthService.Application.Services.TokenService;
using AuthService.Core.DTO;
using AuthService.Core.Models;
using AuthService.Infrastructure.Repositories.UserRepository;
using BuildingBlocks.Common;

namespace AuthService.Application.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, IPasswordService passwordService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<Result> RegisterUserAsync(RegisterUserDto userDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
                return Result.Fail("User already exists");

            var (hash, salt) = await _passwordService.CreatePasswordHashAsync(userDto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Username = userDto.Username,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _userRepository.RegisterUserAsync(user);
            return Result.Ok();
        }


        public async Task<Result<string>> ValidateUserAsync(LoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.EmailAddress);
            if (user == null)
                return Result<string>.Fail("Invalid credentials");

            var isValid = await _passwordService.VerifyPasswordAsync(
                dto.Password,
                user.PasswordHash,
                user.PasswordSalt);

            if (!isValid)
                return Result<string>.Fail("Invalid credentials");

            var token = _tokenService.GenerateToken(user.Id, user.Email);

            return Result<string>.Ok(token);
        }


    }

}


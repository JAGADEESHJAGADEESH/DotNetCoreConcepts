using AuthService.Application.Services.PasswordService;
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

        public UserService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
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


        public async Task<Result> ValidateUserAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.EmailAddress);
            if (user == null)
                return Result.Fail("Invalid credentials");

            var isValid = await _passwordService.VerifyPasswordAsync(
                loginDto.Password,
                user.PasswordHash,
                user.PasswordSalt);

            return isValid
                ? Result.Ok()
                : Result.Fail("Invalid credentials");
        }
    }

}


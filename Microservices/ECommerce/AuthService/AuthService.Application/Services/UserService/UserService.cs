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

        public async Task<Result<UserResponse>> RegisterUserAsync(UserRegistration registration)
        {
            var existingAuthInfo = await GetUserAuthInfoAsync(registration.Email);
            if (existingAuthInfo != null)
                return Result<UserResponse>.Fail("User already exists");

            var user = await MapUserEntity(registration);

            var userId = await _userRepository.RegisterUserAsync(user);
            if (userId != Guid.Empty)
                user.Id = userId;

            var newAuthInfo = new AuthInfo
            {
                Id = user.Id,
                Email = user.Email,
                RoleName = user.Role.RoleName
            };

            var tokenResponse = await GenerateTokenForUserAsync(newAuthInfo);

            var userResponse = await MapUserResponse(user, tokenResponse);

            return Result<UserResponse>.Ok(userResponse);
        }


        public async Task<Result<TokenResponse>> ValidateUserAsync(LoginCredentials credentials)
        {
            var authInfo = await GetUserAuthInfoAsync(credentials.EmailAddress);

            if (authInfo == null)
                return Result<TokenResponse>.Fail("Invalid registration");

            var isValid = await _passwordService.VerifyPasswordAsync(
                credentials.Password,
                authInfo.PasswordHash,
                authInfo.PasswordSalt);

            if (!isValid)
                return Result<TokenResponse>.Fail("Invalid registration");

            var tokenResponse = await GenerateTokenForUserAsync(authInfo);

            return Result<TokenResponse>.Ok(tokenResponse);
        }

        private async Task<Roles> GetUserRoleAsync(int id)
        {
            return await _userRepository.GetRoleByRoleIdAsync(id) ?? new Roles { Id = id, RoleName = "User" };
        }

        private async Task<UserResponse> MapUserResponse(User user, TokenResponse tokenResponse)
        {
            var role = await GetUserRoleAsync(user.Role.Id);
            return new UserResponse
            {
                Id = user.Id,
                UserName = $"{user.FirstName} {user.LastName}",
                EmailAddress = user.Email,
                RoleName = role.RoleName,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                TokenResponse = tokenResponse
            };
        }

        private async Task<User> MapUserEntity(UserRegistration registration)
        {
            var (hash, salt) = await _passwordService.CreatePasswordHashAsync(registration.Password);
            var role = await GetUserRoleAsync(registration.RoleId);
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                Email = registration.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public TokenPayload MapTokenPayload(AuthInfo authInfo)
        {
            return new TokenPayload
            {
                Id = authInfo.Id,
                EmailAddress = authInfo.Email,
                RoleName = authInfo.RoleName
            };
        }

        public async Task<TokenResponse> GenerateTokenForUserAsync(AuthInfo authInfo)
        {
            var tokenPayload = MapTokenPayload(authInfo);
            return await _tokenService.GenerateTokenAsync(tokenPayload);
        }

       public async Task<AuthInfo?> GetUserAuthInfoAsync(string email)
       {
          return await _userRepository.GetUserAuthInfoAsync(email);
       }
    }

}


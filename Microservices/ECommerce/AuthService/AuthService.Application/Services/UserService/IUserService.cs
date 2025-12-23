using AuthService.Core.DTO;
using AuthService.Core.Models;
using BuildingBlocks.Common;

namespace AuthService.Application.Services.UserService
{
    public interface IUserService
    {
        Task<Result<UserResponse>> RegisterUserAsync(UserRegistration registration);
        Task<Result<TokenResponse>> ValidateUserAsync(LoginCredentials credentials);

    }
}

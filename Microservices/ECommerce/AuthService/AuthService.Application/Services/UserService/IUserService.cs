using AuthService.Core.DTO;
using BuildingBlocks.Common;

namespace AuthService.Application.Services.UserService
{
    public interface IUserService
    {
        Task<Result> RegisterUserAsync(RegisterUserDto userDto);
        Task<Result<string>> ValidateUserAsync(LoginDto loginDto);

    }
}

using AuthService.Core.DTO;
using AuthService.Core.Models;

namespace AuthService.Infrastructure.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<Guid> RegisterUserAsync(User user);
        Task<Roles?> GetRoleByRoleIdAsync(int id);
        Task<AuthInfo?> GetUserAuthInfoAsync(string email);
    }
}

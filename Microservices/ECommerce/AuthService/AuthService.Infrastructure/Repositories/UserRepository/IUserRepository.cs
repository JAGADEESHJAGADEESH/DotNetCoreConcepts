using AuthService.Core.DTO;
using AuthService.Core.Models;

namespace AuthService.Infrastructure.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<bool> RegisterUserAsync(User user);
        Task<bool> IsUserEmailExistsInDBAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
    }
}

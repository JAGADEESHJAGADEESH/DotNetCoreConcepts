using Ecommerce.Core.Models;

namespace ECommerce.Infrastructure.UserRepository
{
    public interface IUserRepository
    {
        Task<Users> GetUserByIdAsync(int userId);
        Task<Users> GetUserByUserCredentialsAsync(UserCredentials credentials);
        Task AddUserAsync(Users user);
        Task UpdateUserAsync(Users user);
        Task DeleteUserAsync(int userId);
    }
}

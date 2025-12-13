using Ecommerce.Core.Models;

namespace ECommerce.Infrastructure.UserRepository
{
    public interface IUserRepository
    {
        Task<Users> GetUserByUserCredentialsAsync(string userName);
        Task<int?> SaveUserAsync(Users user);
    }
}

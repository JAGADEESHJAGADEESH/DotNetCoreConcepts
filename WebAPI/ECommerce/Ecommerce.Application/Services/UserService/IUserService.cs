using Ecommerce.Core.Models;

namespace Ecommerce.Application.Services.UserService
{
    public interface IUserService
    {
        Task<Users> GetUserByUserCredentialsAsync(string email);
        Task<int?> SaveUserAsync(Users user);
    }
}

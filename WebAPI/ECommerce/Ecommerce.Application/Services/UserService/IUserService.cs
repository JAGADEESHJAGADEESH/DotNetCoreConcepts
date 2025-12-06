using Ecommerce.Core.Models;

namespace Ecommerce.Application.Services.UserService
{
    public interface IUserService
    {
        Task<Users> GetUserByUserCredentialsAsync(string userName, string passwordSalt);
        Task SaveUserAsync(Users user);
    }
}

using Ecommerce.Core.Models;
using ECommerce.Infrastructure.UserRepository;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public async Task<Users> GetUserByUserCredentialsAsync(string userName, string passwordSalt)
        {
            try
            {
                return await _userRepository.GetUserByUserCredentialsAsync(userName, passwordSalt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user.");
                return null!;
            }
        }

        public async Task SaveUserAsync(Users user)
        {
            try
            {
                await _userRepository.SaveUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user.");
                throw;
            }
        }
    }
}

namespace ECommerce.Infrastructure.TokenRepository
{
    public interface ITokenRepository
    {
        Task SaveTokenAsync(string token, int userId);
        Task<bool> IsTokenValidAsync(string token, int userId);
        Task InvalidateTokenAsync(string token, int userId);
    }
}

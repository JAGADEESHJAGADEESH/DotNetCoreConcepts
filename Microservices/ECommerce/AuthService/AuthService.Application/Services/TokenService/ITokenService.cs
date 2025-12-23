namespace AuthService.Application.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateToken(Guid userId, string email);
    }
}

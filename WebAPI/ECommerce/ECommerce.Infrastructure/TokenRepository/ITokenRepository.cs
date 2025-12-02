using Ecommerce.Core.Models;
using System.Reflection.Metadata;

namespace ECommerce.Infrastructure.TokenRepository
{
    public interface ITokenRepository
    {
        Task<bool> SaveTokenAsync(Token record, CancellationToken cancellationToken);
    }
}

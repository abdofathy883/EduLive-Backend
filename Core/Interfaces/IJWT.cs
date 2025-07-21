using Core.Models;

namespace Core.Interfaces
{
    public interface IJWT
    {
        Task<string> GenerateAccessTokenAsync(BaseUser baseUser);
        Task<RefreshToken> GenerateRefreshTokenAsync();
    }
}

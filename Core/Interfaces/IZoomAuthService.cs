using Core.DTOs;

namespace Core.Interfaces
{
    public interface IZoomAuthService
    {
        string GetAuthorizationUrl();
        Task<ZoomUserConnectionDTO> HandleOAuthCallback(string code, string userId);
        Task<ZoomUserConnectionDTO> GetUserConnectionAsync(string userId);
        Task<bool> RevokeAccessAsync(string userId);
        Task<string> RefreshAccessTokenAsync(string userId);
    }
}

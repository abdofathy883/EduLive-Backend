using Core.DTOs;
using Core.Responses;

namespace Core.Interfaces
{
    public interface IGoogleMeetAuthService
    {
        string GetAuthorizationUrlAsync();
        Task<GoogleMeetAccountDTO> HandleAuthCallbackAsync(string code, string userId);
        Task<GoogleMeetAccountDTO> GetUserConnectionAsync(string userId);
        Task<string> GetAccessTokenAsync(string userId);
        Task<string> RefreshAccessTokenAsync(string userId);
        Task<GoogleMeetAuthResponse> GetUserInfoAsync(string accessToken);

    }
}

using Core.DTOs;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

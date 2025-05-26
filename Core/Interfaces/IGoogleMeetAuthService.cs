using Core.DTOs;
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
        Task<GoogleMeetAccountDTO> GetAccountByUserIdAsync(string userId);
        Task DisconnectAccountAsync(string userId);
        Task<bool> IsAccountConnectedAsync(string userId);
        Task<string> GetAccessTokenAsync(string userId);
        Task<string> RefreshAccessTokenAsync(string userId);
        //Task<bool> RevokeAccessAsync(string userId);
    }
}

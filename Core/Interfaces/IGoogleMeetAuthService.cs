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
        Task<string> GetAuthorizationUrlAsync();
        Task<GoogleMeetAccountDTO> HandleAuthCallbackAsync(string code, int userId);
        Task<GoogleMeetAccountDTO> GetAccountByUserIdAsync(int userId);
        Task DisconnectAccountAsync(int userId);
        Task<bool> IsAccountConnectedAsync(int userId);
        Task<string> GetAccessTokenAsync(int userId);
    }
}

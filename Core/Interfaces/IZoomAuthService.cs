using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IZoomAuthService
    {
        string GetAuthorizationUrl();
        Task<ZoomUserConnectionDTO> HandleOAuthCallback(string code, Guid userId);
        Task<ZoomUserConnectionDTO> GetUserConnectionAsync(Guid userId);
        Task<bool> RevokeAccessAsync(Guid userId);
        Task<string> RefreshAccessTokenAsync(Guid userId);
    }
}

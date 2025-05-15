using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ZoomAuthService : IZoomAuthService
    {
        private readonly HttpClient httpClient;
        //private readonly IOptions<>
        private readonly E_LearningDbContext dbContext;

        public ZoomAuthService(HttpClient _httpClient, E_LearningDbContext context)
        {
            httpClient = _httpClient;
            dbContext = context;
        }
        public string GetAuthorizationUrl()
        {
            //var redirectUrl = _zoomSettings.Value.RedirectUrl;
            //var clientId = _zoomSettings.Value.ClientId;

            var authUrl = "";
                
                //$"https://zoom.us/oauth/authorize" +
                //          $"?response_type=code" +
                //          $"&client_id={clientId}" +
                //          $"&redirect_uri={Uri.EscapeDataString(redirectUrl)}";

            return authUrl;
        }

        public async Task<ZoomUserConnectionDTO> GetUserConnectionAsync(Guid userId)
        {
            var connection = await dbContext.ZoomUserConnections
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (connection == null)
            {
                return new ZoomUserConnectionDTO
                {
                    UserId = userId,
                    IsConnected = false
                };
            }

            // Check if token is expired and needs refresh
            if (connection.TokenExpiry <= DateTime.UtcNow.AddMinutes(5))
            {
                await RefreshAccessTokenAsync(userId);
                // Reload connection after refresh
                connection = await dbContext.ZoomUserConnections
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }

            return new ZoomUserConnectionDTO
            {
                Id = connection.Id,
                UserId = connection.UserId,
                ZoomUserId = connection.ZoomUserId,
                ZoomEmail = connection.ZoomEmail,
                IsConnected = true
            };
        }

        public async Task<ZoomUserConnectionDTO> HandleOAuthCallback(string code, Guid userId)
        {
            var redirectUrl = "";//_zoomSettings.Value.RedirectUrl;
            var clientId = ""; // _zoomSettings.Value.ClientId;
            var clientSecret = ""; // _zoomSettings.Value.ClientSecret;

            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = redirectUrl
            });

            var authHeaderValue = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            httpClient.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Basic", authHeaderValue);

            var tokenResponse = await httpClient.PostAsync(
                "https://zoom.us/oauth/token", tokenRequest);

            tokenResponse.EnsureSuccessStatusCode();

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<ZoomTokenResponse>(tokenContent);

            // Get user info
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenData.access_token);

            var userResponse = await httpClient.GetAsync("https://api.zoom.us/v2/users/me");
            userResponse.EnsureSuccessStatusCode();

            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userData = JsonSerializer.Deserialize<ZoomUserResponse>(userContent);

            var existingConnection = await dbContext.ZoomUserConnections
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (existingConnection != null)
            {
                existingConnection.ZoomUserId = userData.id;
                existingConnection.ZoomEmail = userData.email;
                existingConnection.AccessToken = tokenData.access_token;
                existingConnection.RefreshToken = tokenData.refresh_token;
                existingConnection.TokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.expires_in);
                existingConnection.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
            }
            else
            {
                var newConnection = new ZoomUserConnection
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ZoomUserId = userData.id,
                    ZoomEmail = userData.email,
                    AccessToken = tokenData.access_token,
                    RefreshToken = tokenData.refresh_token,
                    TokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.expires_in),
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.ZoomUserConnections.Add(newConnection);
                await dbContext.SaveChangesAsync();
            }

            return new ZoomUserConnectionDTO
            {
                Id = existingConnection?.Id ?? Guid.NewGuid(),
                UserId = userId,
                ZoomUserId = userData.id,
                ZoomEmail = userData.email,
                IsConnected = true
            };
        }

        public async Task<string> RefreshAccessTokenAsync(Guid userId)
        {
            var connection = await dbContext.ZoomUserConnections
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (connection == null)
                throw new ApplicationException("No Zoom connection found for user");

            //var clientId = _zoomSettings.Value.ClientId;
            //var clientSecret = _zoomSettings.Value.ClientSecret;

            var refreshRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = connection.RefreshToken
            });

            //var authHeaderValue = Convert.ToBase64String(
            //    Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            //httpClient.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Basic", authHeaderValue);

            var response = await httpClient.PostAsync(
                "https://zoom.us/oauth/token", refreshRequest);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<ZoomTokenResponse>(content);

            // Update stored tokens
            connection.AccessToken = tokenData.access_token;
            connection.RefreshToken = tokenData.refresh_token;
            connection.TokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.expires_in);
            connection.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return tokenData.access_token;
        }

        public async Task<bool> RevokeAccessAsync(Guid userId)
        {
            var connection = await dbContext.ZoomUserConnections
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (connection == null) return false;

            //var clientId = _zoomSettings.Value.ClientId;
            //var clientSecret = _zoomSettings.Value.ClientSecret;

            //var authHeaderValue = Convert.ToBase64String(
            //    Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            //httpClient.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Basic", authHeaderValue);

            var revokeRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["token"] = connection.AccessToken
            });

            var response = await httpClient.PostAsync(
                "https://zoom.us/oauth/revoke", revokeRequest);

            // Remove connection from database regardless of Zoom response
            dbContext.ZoomUserConnections.Remove(connection);
            await dbContext.SaveChangesAsync();

            return response.IsSuccessStatusCode;
        }
    }
}

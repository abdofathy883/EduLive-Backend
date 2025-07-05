using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Core.Settings;
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
        private readonly IOptions<ZoomSettings> options;
        private readonly IGenericRepo<ZoomUserAccount> zoomRepo;

        public ZoomAuthService(HttpClient _httpClient, 
            IGenericRepo<ZoomUserAccount> _repo,
            IOptions<ZoomSettings> _options)
        {
            httpClient = _httpClient;
            options = _options;
            zoomRepo = _repo;
        }
        public string GetAuthorizationUrl()
        {
            var redirectUrl = options.Value.RedirectUrl;
            var clientId = options.Value.ClientId;

            var authUrl = $"https://zoom.us/oauth/authorize" +
                      $"?response_type=code" +
                      $"&client_id={clientId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUrl)}";

            return authUrl;
        }

        public async Task<ZoomUserConnectionDTO> GetUserConnectionAsync(string userId)
        {
            var connection = await zoomRepo.GetByIdAsync(userId);

            if (connection == null)
            {
                return new ZoomUserConnectionDTO
                {
                    UserId = userId,
                    ZoomUserId = null,
                    ZoomEmail = null,
                    IsConnected = false
                };
            }

            // Check if token is expired and needs refresh
            if (connection.TokenExpiry <= DateTime.UtcNow.AddMinutes(5))
            {
                await RefreshAccessTokenAsync(userId);
                // Reload connection after refresh
                connection = await zoomRepo.GetByIdAsync(userId);
            }

            return new ZoomUserConnectionDTO
            {
                //Id = connection.Id,
                UserId = connection.UserId,
                ZoomUserId = connection.ZoomUserId,
                ZoomEmail = connection.ZoomEmail,
                IsConnected = true
            };
        }

        public async Task<ZoomUserConnectionDTO> HandleOAuthCallback(string code, string userId)
        {
            var redirectUrl = options.Value.RedirectUrl;
            var clientId = options.Value.ClientId;
            var clientSecret = options.Value.ClientSecret;

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

            var existingConnection = await zoomRepo.GetByIdAsync(userId);

            if (existingConnection != null)
            {
                existingConnection.ZoomUserId = userData.id;
                existingConnection.ZoomEmail = userData.email;
                existingConnection.AccessToken = tokenData.access_token;
                existingConnection.RefreshToken = tokenData.refresh_token;
                existingConnection.TokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.expires_in);
                existingConnection.UpdatedAt = DateTime.UtcNow;

                await zoomRepo.SaveAllAsync();
            }
            else
            {
                var newConnection = new ZoomUserAccount
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

                await zoomRepo.AddAsync(newConnection);
                await zoomRepo.SaveAllAsync();
            }

            return new ZoomUserConnectionDTO
            {
                //Id = existingConnection?.Id ?? Guid.NewGuid(),
                UserId = userId,
                ZoomUserId = userData.id,
                ZoomEmail = userData.email,
                IsConnected = true
            };
        }

        public async Task<string> RefreshAccessTokenAsync(string userId)
        {
            var connection = await zoomRepo.GetByIdAsync(userId);

            if (connection == null)
                throw new ApplicationException("No Zoom connection found for user");

            var clientId = options.Value.ClientId;
            var clientSecret = options.Value.ClientSecret;

            var refreshRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = connection.RefreshToken
            });

            var authHeaderValue = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authHeaderValue);

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

            await zoomRepo.SaveAllAsync();

            return tokenData.access_token;
        }

        public async Task<bool> RevokeAccessAsync(string userId)
        {
            var connection = await zoomRepo.GetByIdAsync(userId);

            if (connection == null) return false;

            var clientId = options.Value.ClientId;
            var clientSecret = options.Value.ClientSecret;

            var authHeaderValue = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authHeaderValue);

            var revokeRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["token"] = connection.AccessToken
            });

            var response = await httpClient.PostAsync(
                "https://zoom.us/oauth/revoke", revokeRequest);

            // Remove connection from database regardless of Zoom response
            await zoomRepo.DeleteByIdAsync(connection.Id);
            await zoomRepo.SaveAllAsync();

            return response.IsSuccessStatusCode;
        }
    }
}

using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Core.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class GoogleMeetAuthService : IGoogleMeetAuthService
    {
        private readonly IGenericRepo<GoogleMeetUserAccount> repo;
        private readonly IHttpClientFactory httpClient;
        private readonly ILogger<GoogleMeetAuthService> logger;
        private readonly IOptions<GoogleSettings> meetSettings;

        public GoogleMeetAuthService(IGenericRepo<GoogleMeetUserAccount> genericRepo, 
            IHttpClientFactory httpClientFactory, 
            ILogger<GoogleMeetAuthService> _logger, 
            IOptions<GoogleSettings> options)
        {
            repo = genericRepo;
            httpClient = httpClientFactory;
            logger = _logger;
            meetSettings = options;
        }

        public async Task<string> GetAccessTokenAsync(string userId)
        {
            var account = await repo.GetByIdAsync(userId);

            if (account == null)
                throw new InvalidOperationException("No connected Google Meet account for this user.");

            // Check if token needs refresh
            if (account.TokenExpiry <= DateTime.UtcNow.AddMinutes(5))
            {
                return await RefreshAccessTokenAsync(userId);
            }

            return account.AccessToken;
        }

        public async Task<GoogleMeetAccountDTO> GetUserConnectionAsync(string userId)
        {
            var account = await repo.GetByIdAsync(userId);

            if (account == null)
            {
                throw new ArgumentNullException(userId, "No connected Google Meet account for this user.");
            }

            if (account.TokenExpiry <= DateTime.UtcNow.AddMinutes(5))
            {
                await RefreshAccessTokenAsync(userId);
                account = await repo.GetByIdAsync(userId);
            }

            return new GoogleMeetAccountDTO
            {
                Id = account.Id,
                UserId = account.UserId,
                Email = account.Email,
                GoogleUserId = account.GoogleUserId,
                IsConnected = true
            };
        }

        public string GetAuthorizationUrl()
        {
            var clientId = meetSettings.Value.ClientId;
            var redirectUri = meetSettings.Value.RedirectUrl;
            var scope = "https://www.googleapis.com/auth/calendar";

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                          $"client_id={clientId}&" +
                          $"redirect_uri={redirectUri}&" +
                          $"response_type=code&" +
                          $"scope={scope}&" +
                          $"access_type=offline&" +
                          $"prompt=consent";

            return authUrl;
        }

        public async Task<GoogleMeetAccountDTO> HandleAuthCallbackAsync(string code, string userId)
        {
            var Tokens = await ExchangeCodeForTokensAsync(code);
            var userInfo = await GetUserInfoAsync(Tokens.AccessToken);

            var existingAccount = await repo.GetByIdAsync(userId);

            if (existingAccount != null)
            {
                // Update existing account
                existingAccount.GoogleUserId = userInfo.GoogleUserId;
                existingAccount.Email = userInfo.Email;
                existingAccount.AccessToken = Tokens.AccessToken;
                existingAccount.RefreshToken = Tokens.RefreshToken;
                existingAccount.TokenExpiry = DateTime.UtcNow.AddSeconds(Tokens.ExpiresIn);
                existingAccount.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new account
                existingAccount = new GoogleMeetUserAccount
                {
                    UserId = userId.ToString(),
                    GoogleUserId = userInfo.GoogleUserId,
                    Email = userInfo.Email,
                    AccessToken = Tokens.AccessToken,
                    RefreshToken = Tokens.RefreshToken,
                    TokenExpiry = DateTime.UtcNow.AddSeconds(Tokens.ExpiresIn),
                    CreatedAt = DateTime.UtcNow,
                };
                await repo.AddAsync(existingAccount);
            }

            await repo.SaveAllAsync();

            return new GoogleMeetAccountDTO
            {
                Id = existingAccount.Id,
                UserId = existingAccount.UserId,
                Email = existingAccount.Email,
                GoogleUserId = existingAccount.GoogleUserId,
                IsConnected = true
            };
        }

        public async Task<string> RefreshAccessTokenAsync(string userId)
        {
            var account = await repo.GetByIdAsync(userId);
            if (account == null)
            {
                throw new InvalidOperationException($"No Google Account was found for user: {userId}");
            }

            var clientId = meetSettings.Value.ClientId;
            var clientSecret = meetSettings.Value.ClientSecret;
            var http = httpClient.CreateClient();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("refresh_token", account.RefreshToken),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            });

            var response = await http.PostAsync("https://oauth2.googleapis.com/token", formContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<GoogleMeetAuthResponse>(content);

            account.AccessToken = token.AccessToken;
            if (!string.IsNullOrEmpty(token.RefreshToken))
            {
                account.RefreshToken = token.RefreshToken;
            }
            account.TokenExpiry = DateTime.UtcNow.AddSeconds(token.ExpiresIn);
            account.UpdatedAt = DateTime.UtcNow;

            //await repo.Update(account);
            await repo.SaveAllAsync();

            return account.AccessToken;
        }

        private async Task<GoogleMeetAuthResponse> ExchangeCodeForTokensAsync(string code)
        {
            var clientId = meetSettings.Value.ClientId;
            var clientSecret = meetSettings.Value.ClientSecret;
            var redirectUri = meetSettings.Value.RedirectUrl;

            var http = httpClient.CreateClient();

            var formContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
            });

            var response = await http.PostAsync("https://oauth2.googleapis.com/token", formContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GoogleMeetAuthResponse>(content);
        }

        public async Task<GoogleMeetAuthResponse> GetUserInfoAsync(string accessToken)
        {
            var http = httpClient.CreateClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await http.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<GoogleMeetAuthResponse>(content);

            return userInfo;
        }
    }
}

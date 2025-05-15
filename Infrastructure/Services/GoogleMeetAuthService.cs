using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IGenericRepo<GoogleMeetAccount> repo;
        private readonly IHttpClientFactory httpClient;
        private readonly ILogger<GoogleMeetAuthService> logger;

        public GoogleMeetAuthService(IGenericRepo<GoogleMeetAccount> genericRepo, IHttpClientFactory httpClientFactory, ILogger<GoogleMeetAuthService> _logger)
        {
            repo = genericRepo;
            httpClient = httpClientFactory;
            logger = _logger;
        }
        public async Task DisconnectAccountAsync(int userId)
        {
            var account = await repo.GetByIdAsync(userId);

            if (account is not null)
            {
                await repo.DeleteByIdAsync(account.Id);
                await repo.SaveAllAsync();
            }
        }

        public async Task<string> GetAccessTokenAsync(int userId)
        {
            var account = await repo.GetByIdAsync(userId);
            //var account = await _context.GoogleMeetAccounts
            //.FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                return null;
            }

            // Check if token needs refresh
            if (account.TokenExpiry <= DateTime.UtcNow.AddMinutes(5))
            {
                // Refresh token
                var refreshedToken = await RefreshAccessTokenAsync(account.RefreshToken);

                account.AccessToken = refreshedToken.AccessToken;
                account.TokenExpiry = DateTime.UtcNow.AddSeconds(refreshedToken.ExpiresIn);
                //await _context.SaveChangesAsync();
                await repo.SaveAllAsync();
            }

            return account.AccessToken;
        }

        public async Task<GoogleMeetAccountDTO> GetAccountByUserIdAsync(int userId)
        {
            var account = await repo.GetByIdAsync(userId);

            if (account == null)
            {
                return new GoogleMeetAccountDTO
                {
                    IsConnected = false
                };
            }

            return new GoogleMeetAccountDTO
            {
                Id = account.Id,
                Email = account.Email,
                IsConnected = true
            };
        }

        public Task<string> GetAuthorizationUrlAsync()
        {
            var clientId = ""; // _configuration["GoogleMeet:ClientId"];
            var redirectUri = ""; // _configuration["GoogleMeet:RedirectUri"];
            var scope = "https://www.googleapis.com/auth/calendar";

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                          $"client_id={clientId}&" +
                          $"redirect_uri={redirectUri}&" +
                          $"response_type=code&" +
                          $"scope={scope}&" +
                          $"access_type=offline&" +
                          $"prompt=consent";

            return Task.FromResult(authUrl);
        }

        public async Task<GoogleMeetAccountDTO> HandleAuthCallbackAsync(string code, int userId)
        {
            var tokenResponse = await ExchangeCodeForTokensAsync(code);
            var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);

            var existingAccount = await repo.GetByIdAsync(userId);
            //var existingAccount = allAccounts
            //    //await _context.GoogleMeetAccounts
            //    .FirstOrDefaultAsync(a => a.UserId == userId);

            if (existingAccount != null)
            {
                // Update existing account
                existingAccount.GoogleUserId = userInfo.GoogleUserId;
                existingAccount.Email = userInfo.Email;
                existingAccount.AccessToken = tokenResponse.AccessToken;
                existingAccount.RefreshToken = tokenResponse.RefreshToken;
                existingAccount.TokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            }
            else
            {
                // Create new account
                existingAccount = new GoogleMeetAccount
                {
                    UserId = userId,
                    GoogleUserId = userInfo.GoogleUserId,
                    Email = userInfo.Email,
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    TokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
                };
                await repo.AddAsync(existingAccount);
                //_context.GoogleMeetAccounts.Add(existingAccount);
            }

            await repo.SaveAllAsync();

            return new GoogleMeetAccountDTO
            {
                Id = existingAccount.Id,
                Email = existingAccount.Email,
                IsConnected = true
            };
        }

        public async Task<bool> IsAccountConnectedAsync(int userId)
        {
            var isAccountConnected = await repo.GetByIdAsync(userId);
            if (isAccountConnected is null)
            {
                return false;
            }
            return true;
        }

        private async Task<GoogleMeetAuthResponse> ExchangeCodeForTokensAsync(string code)
        {
            var clientId = ""; // _configuration["GoogleMeet:ClientId"];
            var clientSecret = ""; //_configuration["GoogleMeet:ClientSecret"];
            var redirectUri = ""; // _configuration["GoogleMeet:RedirectUri"];

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
            var token = JsonSerializer.Deserialize<GoogleMeetAuthResponse>(content);

            return token;
        }

        private async Task<GoogleMeetAuthResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            var clientId = ""; //_configuration["GoogleMeet:ClientId"];
            var clientSecret = ""; // _configuration["GoogleMeet:ClientSecret"];

            var http = httpClient.CreateClient();

            var formContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "refresh_token")
        });

            var response = await http.PostAsync("https://oauth2.googleapis.com/token", formContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<GoogleMeetAuthResponse>(content);

            return token;
        }

        private async Task<GoogleMeetAuthResponse> GetUserInfoAsync(string accessToken)
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

using Core.Interfaces;
using Core.Models;
using Infrastructure.Configrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class JWTService : IJWT
    {
        private readonly UserManager<BaseUser> userManager;
        private readonly IOptions<JwtSettings> jwtSettings;
        public JWTService(UserManager<BaseUser> manager, IOptions<JwtSettings> jwt)
        {
            userManager = manager;
            jwtSettings = jwt;
        }
        public async Task<string> GenerateAccessTokenAsync(BaseUser baseUser)
        {
            var userClaims = await userManager.GetClaimsAsync(baseUser);
            var userRoles = await userManager.GetRolesAsync(baseUser);
            var roleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub, baseUser.UserName??""),
            new (JwtRegisteredClaimNames.Email, baseUser.Email ?? ""),
            new ("uid", baseUser.Id)
        }.Union(userClaims)
             .Union(roleClaims);

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key));

            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwtSettings.Value.Issuer,
                audience: jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpirationMinutes + 60),
                signingCredentials: signingCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return accessToken;
        }

        public Task<RefreshToken> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Task.FromResult(new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                CreateOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays)
            });
        }
    }
}

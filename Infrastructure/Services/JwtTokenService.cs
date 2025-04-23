using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.Interface;
using Core.Models;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class JwtTokenService(UserManager<AppUser> userManager, JwtSettings jwtSetting) : IJwtTokenService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly JwtSettings _jwtSetting = jwtSetting;
        public async Task<string> GenerateAccessTokenAsync(AppUser appUser)
        {
            var userClaims = await _userManager.GetClaimsAsync(appUser);
            var userRoles = await _userManager.GetRolesAsync(appUser);
            var roleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub, appUser.UserName??""),
            new (JwtRegisteredClaimNames.Email, appUser.Email ?? ""),
            new ("uid", appUser.Id)
        }.Union(userClaims)
             .Union(roleClaims);

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key));

            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSetting.ExpirationMinutes),
                signingCredentials: signingCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return accessToken;
        }

        public Task<RefreshTocken> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Task.FromResult(new RefreshTocken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    CreateOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationDays)
                });
            }
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    public Task<(string accessToken, string refreshToken)> AuthenticateAsync(AppUser appUser)
    {
        throw new NotImplementedException();
    }

    public Task<(string newAccessToken, string newRefreshToken)> RefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }
}

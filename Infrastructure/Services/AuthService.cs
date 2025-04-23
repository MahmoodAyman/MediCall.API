using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.DTOs;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService (UserManager<AppUser> userManager , IJwtTokenService jwtTokenService) : IAuthService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    public async Task<AuthDTO> LoginAsync(LoginDTO loginDTO)
    {
        var authDTO = new AuthDTO();
        var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        if (user is null ||await _userManager.CheckPasswordAsync(user,loginDTO.Password))
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage="Email or password is incorrect";
            return authDTO;
        }       

        authDTO.IsAuthenticated = true;
        authDTO.Email = user.Email;
        authDTO.UserName = user.UserName;
        authDTO.Token = await _jwtTokenService.GenerateAccessTokenAsync(user);
        authDTO.Roles = [.. (await _userManager.GetRolesAsync(user))];
        
        if (user.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = user.RefreshTokens.First(t => t.IsActive);
            authDTO.RefreshToken = activeRefreshToken.Token;
            authDTO.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();
            authDTO.RefreshToken = refreshToken.Token;
            authDTO.RefreshTokenExpiration = refreshToken.ExpiresOn;
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }

        authDTO.Massage = "Login successfully";
        return authDTO;
    }

    public Task<AuthDTO> RefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<AuthDTO> RegisterAsync(RegisterDTO registerDTO)
    {
        throw new NotImplementedException();
    }
}

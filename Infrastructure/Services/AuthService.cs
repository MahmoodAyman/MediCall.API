using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Core.DTOs;
using Core.Enums;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public partial class AuthService(UserManager<AppUser> userManager, IJwtTokenService jwtTokenService, IMailingService mailingService) : IAuthService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IMailingService _mailingService = mailingService;
    public async Task<AuthDTO> LoginAsync(LoginDTO loginDTO)
    {
        var authDTO = new AuthDTO();
        var user = await _userManager.FindByEmailAsync(loginDTO.Email);

        if (user is null ||! await _userManager.CheckPasswordAsync(user,loginDTO.Password))
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "Email or password is incorrect";
            return authDTO;
        }

        if(!await _userManager.IsEmailConfirmedAsync(user))
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "The email is not confirmed. Check your inbox.";
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

    public async Task<AuthDTO> RefreshTokenAsync(string token)
    {
        var authDTO = new AuthDTO();

        var user = _userManager.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        if (user is null)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "Invalid refresh token";
            return authDTO;
        }
        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if (!refreshToken.IsActive)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "Inactive refresh token";
            return authDTO;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;
        var newRefreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();

        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        authDTO.IsAuthenticated = true;
        authDTO.Email = user.Email;
        authDTO.UserName = user.UserName;
        authDTO.Token = await _jwtTokenService.GenerateAccessTokenAsync(user);
        authDTO.Roles = [.. (await _userManager.GetRolesAsync(user))];
        authDTO.RefreshToken = newRefreshToken.Token;
        authDTO.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

        return authDTO;
    }

    public async Task<AuthDTO> RegisterAsync(RegisterDTO registerDTO)
    {
        var authDTO = new AuthDTO();

        var validateErrors = ValidateRegisterDTO(registerDTO);
        if (validateErrors is not null && validateErrors.Count > 0)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = string.Empty;
            foreach (var error in validateErrors)
            {
                authDTO.Massage += error + " , ";
            }
            return authDTO;
        }

        if (await _userManager.FindByIdAsync(registerDTO.NationalId) is not null)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "National ID already exists";
            return authDTO;
        }
        if (await _userManager.FindByEmailAsync(registerDTO.Email) is not null)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "Email already exists";
            return authDTO;
        }
        if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == registerDTO.PhoneNumber))
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "Phone number already exists";
            return authDTO;
        }
        if (await _userManager.Users.AnyAsync(u => u.UserName == registerDTO.UserName))
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "User name already exists";
            return authDTO;
        }


        var user = new AppUser
        {
            Id= registerDTO.NationalId,
            Email= registerDTO.Email,
            UserName = registerDTO.UserName,
            PhoneNumber = registerDTO.PhoneNumber,
            FirstName= registerDTO.FirstName,
            LastName= registerDTO.LastName,
            DateOfBirth= registerDTO.DateOfBirth,
            Gender= registerDTO.Gender,
            Location= registerDTO.Location
        };

        var result = await _userManager.CreateAsync(user, registerDTO.Password);
        if (!result.Succeeded)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = string.Empty;
            foreach (var error in result.Errors)
            {
                authDTO.Massage += error.Description + " , ";
            }
            return authDTO;
        }

        await _userManager.AddToRoleAsync(user, registerDTO.Role.ToString());

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var param = new Dictionary<string, string?>
        {
            {"token", token },
            {"email", user.Email}
        };

        var callbackUrl = QueryHelpers.AddQueryString("https://localhost:7116/api/Auth/ConfirmEmail", param);

        await _mailingService.SendEmailAsync(user.Email, "Confirm your email", $"<a href=\"{callbackUrl}\">Confirm From Here</a>", null);

        authDTO.IsAuthenticated = true;
        authDTO.Massage = "Register Successfully. Check your inbox to confirm your mail";
        return authDTO;

    }
    private static List<string> ValidateRegisterDTO(RegisterDTO dto)
    {
        var errors = new List<string>();

        // === National ID Validation ===
        if (string.IsNullOrWhiteSpace(dto.NationalId) || dto.NationalId.Length != 14 || !dto.NationalId.All(char.IsDigit))
        {
            errors.Add("National ID must be exactly 14 digits.");
        }
        else
        {
            string centuryCode = dto.NationalId[..1];
            string year = dto.NationalId.Substring(1, 2);
            string month = dto.NationalId.Substring(3, 2);
            string day = dto.NationalId.Substring(5, 2);

            int fullYear;
            if (centuryCode == "2") fullYear = 1900 + int.Parse(year);
            else if (centuryCode == "3") fullYear = 2000 + int.Parse(year);
            else
            {
                errors.Add("Invalid century code in National ID.");
                return errors;
            }

            if (int.TryParse(month, out int monthInt) && int.TryParse(day, out int dayInt))
            {
                try
                {
                    var idDate = new DateTime(fullYear, monthInt, dayInt);
                    if (dto.DateOfBirth.Date != idDate)
                    {
                        errors.Add("Date of birth does not match National ID.");
                    }
                }
                catch
                {
                    errors.Add("Invalid date of birth in National ID.");
                }
            }
            else
            {
                errors.Add("Month or day in National ID is not valid.");
            }

            int genderDigit = int.Parse(dto.NationalId[12].ToString());
            var genderFromId = (genderDigit % 2 == 0) ? Gender.Female : Gender.Male;
            if (dto.Gender != genderFromId)
            {
                errors.Add("Gender does not match National ID.");
            }
        }

        // === Email Validation ===
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            !EmailRegex().IsMatch(dto.Email))
        {
            errors.Add("Invalid email format.");
        }

        // === Password Validation ===
        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            errors.Add("Password is required.");
        }
        else if (dto.Password.Length < 6)
        {
            errors.Add("Password must be at least 6 characters.");
        }

        // === Confirm Password Match ===
        if (dto.Password != dto.ConfirmPassword)
        {
            errors.Add("Passwords do not match.");
        }

        // === Phone Number Validation ===
        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            errors.Add("Phone number is required.");
        }
        else if (!PhoneRegex().IsMatch(dto.PhoneNumber))
        {
            errors.Add("Invalid Egyptian phone number.");
        }

        // === Name Fields ===
        if (string.IsNullOrWhiteSpace(dto.FirstName)) errors.Add("First name is required.");
        if (string.IsNullOrWhiteSpace(dto.LastName)) errors.Add("Last name is required.");

        // === Location Validation ===
        if (dto.Location == null)
        {
            errors.Add("Location is required.");
        }

        return errors;
    }

    public async Task<AuthDTO> ConfirmEmailAsync(string email, string token)
    {
        var authDTO = new AuthDTO();
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "User not found";
            return authDTO;
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Massage = "Error confirming email";
            return authDTO;
        }
        authDTO.IsAuthenticated = true;
        authDTO.Massage = "Email confirmed successfully";
        return authDTO;
    }

    [GeneratedRegex(@"^01[0-2,5]{1}[0-9]{8}$")]
    private static partial Regex PhoneRegex();
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex EmailRegex();
}

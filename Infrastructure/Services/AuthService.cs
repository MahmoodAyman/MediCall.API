using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Core.DTOs;
using Core.Enums;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public partial class AuthService(
    UserManager<AppUser> userManager,
    IJwtTokenService jwtTokenService,
    IMailingService mailingService,
    IUploadFileService uploadFile,
    IHttpContextAccessor httpContextAccessor,
    IGenericRepository<Certificate> certificateRepository
    ) : IAuthService

{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IMailingService _mailingService = mailingService;
    private readonly IUploadFileService _uploadFileService = uploadFile;
    private readonly IGenericRepository<Certificate> _certificateRepository = certificateRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<AuthDTO> LoginAsync(LoginDTO loginDTO)
    {
        var authDTO = new AuthDTO();
        var user = await _userManager.FindByEmailAsync(loginDTO.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
        {
            authDTO.IsAuthenticated = false;
            authDTO.Message = "Email or password is incorrect";
            return authDTO;
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            authDTO.IsAuthenticated = false;
            authDTO.Message = "The email is not confirmed. Check your inbox.";
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

        authDTO.Message = "Login successfully";
        return authDTO;
    }

    public async Task<AuthDTO> RefreshTokenAsync(string token)
    {
        var authDTO = new AuthDTO();

        var user = _userManager.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        if (user is null)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Message = "Invalid refresh token";
            return authDTO;
        }
        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if (!refreshToken.IsActive)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Message = "Inactive refresh token";
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
    public async Task<AuthDTO> PatientRegisterAsync(RegisterDTO registerDTO)
    {

        var validateErrors = await ValidateRegisterDTOAsync(registerDTO);
        if (validateErrors is not null && validateErrors.Count > 0)
        {
            return FailResult(string.Join(", ", validateErrors));
        }
        string? imagePath = null;
        if (registerDTO.Image is not null)
        {
            imagePath = await _uploadFileService.UploadFile(registerDTO.Image);
        }

        var user = new Patient
        {
            Id = registerDTO.NationalId,
            Email = registerDTO.Email,
            UserName = registerDTO.Email.Split('@')[0],
            PhoneNumber = registerDTO.PhoneNumber,
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            DateOfBirth = registerDTO.DateOfBirth,
            Gender = registerDTO.Gender,
            Location = registerDTO.Location,
            ProfilePicture = imagePath
        };

        var result = await _userManager.CreateAsync(user, registerDTO.Password);


        await _userManager.AddToRoleAsync(user, Role.Patient.ToString());

        if (!result.Succeeded)
        {
            return FailResult(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await SendConfirmationEmail(user);

        var authDTO = new AuthDTO
        {
            IsAuthenticated = true,
            Message = "Register Successfully. Check your inbox to confirm your mail"
        };

        return authDTO;
    }
    public async Task<AuthDTO> NurseRegisterAsync(NurseRegisterDTO registerDTO)
    {
        var validateErrors = await ValidateNurseRegisterDTOAsync(registerDTO);
        if (validateErrors is not null && validateErrors.Count > 0)
        {
            return FailResult(string.Join(", ", validateErrors));
        }

        string? imagePath = null;
        if (registerDTO.Image is not null)
        {
            imagePath = await _uploadFileService.UploadFile(registerDTO.Image);
        }

        var nurseCertificates = new List<NurseCertificate>();
        foreach (var certificateDTO in registerDTO.NurseCertificates)
        {

            var nurseCertificate = new NurseCertificate
            {
                NurseId = registerDTO.NationalId,
                CertificateId = certificateDTO.CertificateId,
                FilePath = await _uploadFileService.UploadFile(certificateDTO.File),
                ExpirationDate = certificateDTO.ExpirationDate
            };
            nurseCertificates.Add(nurseCertificate);
        }

        var user = new Nurse
        {
            Id = registerDTO.NationalId,
            Email = registerDTO.Email,
            UserName = registerDTO.UserName,
            PhoneNumber = registerDTO.PhoneNumber,
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            DateOfBirth = registerDTO.DateOfBirth,
            Gender = registerDTO.Gender,
            Location = registerDTO.Location,
            ExperienceYears = registerDTO.ExperienceYears,
            LicenseNumber = registerDTO.LicenseNumber,
            ProfilePicture = imagePath,
            Certificates = nurseCertificates
        };

        var result = await _userManager.CreateAsync(user, registerDTO.Password);
        if (!result.Succeeded)
        {
            FailResult(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, Role.Nurse.ToString());
        await SendConfirmationEmail(user);

        var authDTO = new AuthDTO
        {
            IsAuthenticated = true,
            Message = "Register Successfully. Check your inbox to confirm your mail"
        };

        return authDTO;
    }
    private async Task SendConfirmationEmail(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var param = new Dictionary<string, string?>
        {
            {"token", token },
            {"email", user.Email}
        };

        var request = _httpContextAccessor.HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var callbackUrl = QueryHelpers.AddQueryString($"{baseUrl}/api/Auth/ConfirmEmail", param);
        string emailBody = $@"
        <html>
            <body style=""font-family: Arial, sans-serif;"">
                <h2>Welcome {user.FirstName}!</h2>
                <p>Thanks for registering. Please confirm your email by clicking the link below:</p>
                <a href=""{callbackUrl}"" style=""padding: 10px 20px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px;"">Confirm Email</a>
                <p>If you did not request this, please ignore this email.</p>
            </body>
        </html>";

        await _mailingService.SendEmailAsync(user.Email ?? throw new NullReferenceException("Eail is null here"), "Confirm your email", emailBody, null);
    }
    public async Task<AuthDTO> ConfirmEmailAsync(string email, string token)
    {
        var authDTO = new AuthDTO();
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Message = "User not found";
            return authDTO;
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            authDTO.IsAuthenticated = false;
            authDTO.Message = "Error confirming email";
            return authDTO;
        }
        authDTO.IsAuthenticated = true;
        authDTO.Message = "Email confirmed successfully";
        return authDTO;
    }

    private async Task<List<string>> ValidateRegisterDTOAsync(RegisterDTO dto)
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

        // === image validation ===
        if (dto.Image is not null)
        {
            if (!IsFileValid(dto.Image))
            {
                errors.Add("Image is not valid. Allowed formats: jpg, jpeg, png. Max size: 2MB.");
            }
        }

        // === Date of Birth Validation ===
        if (dto.DateOfBirth == default)
        {
            errors.Add("Date of birth is required.");
        }
        else if (dto.DateOfBirth > DateTime.Now.AddYears(-18))
        {
            errors.Add("You must be at least 18 years old to register.");
        }
        else if (dto.DateOfBirth < DateTime.Now.AddYears(-120))
        {
            errors.Add("Date of birth is not valid.");
        }
        // === Unique Fields Validation ===
        if (await _userManager.FindByIdAsync(dto.NationalId) is not null)
        {
            errors.Add("National ID already exists");
        }
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
        {
            errors.Add("Email already exists");
        }
        if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber))
        {
            errors.Add("Phone number already exists");
        }
        if (await _userManager.Users.AnyAsync(u => u.UserName == dto.UserName))
        {
            errors.Add("User name already exists");
        }

        return errors;
    }
    private async Task<List<string>> ValidateNurseRegisterDTOAsync(NurseRegisterDTO dto)
    {
        var errors = await ValidateRegisterDTOAsync(dto);
        if (dto.ExperienceYears < 0)
        {
            errors.Add("Experience years must be a positive number.");
        }
        if (dto.ExperienceYears > DateTime.Now.Year - dto.DateOfBirth.Year)
        {
            errors.Add("Experience years must be less than age.");
        }
        if (string.IsNullOrWhiteSpace(dto.LicenseNumber))
        {
            errors.Add("License number is required.");
        }

        if (dto.NurseCertificates is null || dto.NurseCertificates.Count == 0)
        {
            errors.Add("At least one certificate is required.");
        }
        else
        {
            foreach (var certificate in dto.NurseCertificates)
            {
                errors.AddRange(await NurseCertificateValidationAsync(certificate));
            }
        }
        return errors;
    }
    private async Task<List<string>> NurseCertificateValidationAsync(NurseCertificateDTO dto)
    {
        var errors = new List<string>();
        var cert = await _certificateRepository.GetByIdAsync(dto.CertificateId);
        if (cert is null)
        {
            errors.Add($"Certificate with ID {dto.CertificateId} does not exist.");
            return errors;
        }
        if (!IsFileValid(dto.File))
        {
            errors.Add($"Certificate file for {cert.Name} is not valid. Allowed formats: jpg, jpeg, png. Max size: 2MB.");
        }
        if (cert.IsExpirable && dto.ExpirationDate == default)
        {
            errors.Add($"Expiration date is required for {cert.Name}.");
        }
        return errors;
    }
    private static bool IsFileValid(IFormFile file)
    {
        if (file is null) return true;
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(fileExtension))
        {
            return false;
        }
        if (file.Length > 2 * 1024 * 1024)
        {
            return false;
        }
        return true;
    }

    private static AuthDTO FailResult(string message)
    {
        return new AuthDTO()
        {
            IsAuthenticated = false,
            Message = message
        };
    }

    [GeneratedRegex(@"^01[0-2,5]{1}[0-9]{8}$")]
    private static partial Regex PhoneRegex();
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex EmailRegex();
    public async Task<AppUser> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        var user = await _userManager.FindByEmailAsync(email);
        return user;
    }

}

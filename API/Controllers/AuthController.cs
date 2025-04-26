using System.Threading.Tasks;
using Core.DTOs;
using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController (IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService  = authService;
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDTO);
            if(!result.IsAuthenticated)
            {
                return BadRequest(new { result.Message });
            }
            
            if(result.RefreshToken is not null)
            {
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }


            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDTO);

            if (!result.IsAuthenticated)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(new { result.Message });
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { Message = "No refresh token provided" });
            }
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
            {
                return BadRequest(new { result.Message });
            }
            if (result.RefreshToken is not null)
            {
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }
            return Ok(result);
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email , [FromQuery] string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ConfirmEmailAsync(email,token);
            if (!result.IsAuthenticated)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new { result.Message });
        }

        private void SetRefreshTokenCookie(string refreshToken , DateTime Expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = Expires
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}

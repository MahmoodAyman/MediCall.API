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
                return BadRequest(new { Message = result.Massage });
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
                return BadRequest(new { Message = result.Massage });
            }

            return Ok(new { Message = result.Massage });
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
